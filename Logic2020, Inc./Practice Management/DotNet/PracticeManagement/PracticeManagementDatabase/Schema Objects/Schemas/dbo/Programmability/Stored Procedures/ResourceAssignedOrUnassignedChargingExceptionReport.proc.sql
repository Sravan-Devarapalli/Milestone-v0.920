CREATE PROCEDURE [dbo].[ResourceAssignedOrUnassignedChargingExceptionReport]
	(
		@StartDate			DATETIME,
		@EndDate			DATETIME,
		@IsUnassignedReport BIT = 0
	)
AS
BEGIN
		DECLARE @W2Hourly	INT,
				@W2Salary	INT,
				@1099Hourly	INT
	
		SELECT @W2Hourly = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
		SELECT @W2Salary = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
		SELECT @1099Hourly = TimescaleId FROM dbo.Timescale WHERE Name = '1099 Hourly'

		DECLARE @FutureDate DATETIME
		SELECT @FutureDate = dbo.GetFutureDate()
		;WITH PersonForeCastedHours
					AS
					(	
						SELECT  MP.PersonId ,
								SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ForecastedHours,
								Pro.ProjectId,
								Pro.ProjectNumber,
								pay.Timescale,
								Pro.ProjectStatusId,
								Pro.Name AS ProjectName,
								PS.Name AS ProjectStatus
						FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
								INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
														AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate	
								INNER JOIN dbo.Project Pro ON Pro.ProjectId = M.ProjectId
								INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId
								INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = Pro.ProjectStatusId
						WHERE   PC.Date BETWEEN @StartDate AND @EndDate 
								AND Pro.IsAllowedToShow = 1 AND Pro.ProjectNumber != 'P031000'
								AND Pro.ProjectStatusId IN (3,4) --Active AND Completed status
								AND PC.Date BETWEEN pay.StartDate AND pay.EndDateOrig-1
								AND pay.Timescale IN (@W2Hourly,@W2Salary,@1099Hourly)
						GROUP BY	MP.PersonId,
									Pro.ProjectId,
									pay.Timescale,
									Pro.ProjectNumber,
									Pro.ProjectStatusId,
									PS.Name,
									Pro.Name 
					),
					PersonTimeEntries
					AS
					(
						SELECT	TE.PersonId,
								Pro.ProjectId,
								pay.Timescale,
								Pro.ProjectNumber,
								Pro.ProjectStatusId,
								Pro.Name AS ProjectName,
								PS.Name AS ProjectStatus,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
												THEN TEH.ActualHours
												ELSE 0
											END), 2) AS BillableHours ,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 
												THEN TEH.ActualHours
												ELSE 0
											END), 2) AS NonBillableHours
						FROM	TimeEntry TE
								INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId 
								INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
																		AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
								INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId AND P.IsStrawman = 0
																AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
								INNER JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId 
								INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId
								INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = Pro.ProjectStatusId
						WHERE	CC.TimeEntrySectionId != 4 -- Adminstrative TimeEntrySectionId
										AND Pro.IsAllowedToShow = 1 AND Pro.ProjectNumber != 'P031000'
										AND Pro.ProjectStatusId IN (3,4)
										AND TE.ChargeCodeDate BETWEEN pay.StartDate AND pay.EndDateOrig-1
										AND pay.Timescale IN (@W2Hourly,@W2Salary,@1099Hourly)
						GROUP BY	TE.PersonId,
									Pro.ProjectId,
									pay.Timescale,
									Pro.ProjectNumber,
									Pro.ProjectStatusId,
									Pro.Name,
									PS.Name
					)
					SELECT	ISNULL(PFH.PersonId,PTE.PersonId) AS PersonId,
							P.EmployeeNumber,
							P.FirstName,
							p.LastName,
							P.IsOffshore,
							TS.Name AS TimeScaleName,
							ISNULL(PFH.ProjectId,PTE.ProjectId) AS ProjectId,
							ISNULL(PFH.ProjectNumber,PTE.ProjectNumber) AS ProjectNumber,
							ISNULL(PFH.ProjectStatusId,PTE.ProjectStatusId) AS ProjectStatusId,
							ISNULL(PFH.ProjectName,PTE.ProjectName) AS ProjectName,
							ISNULL(PFH.ForecastedHours,0) AS ForecastedHours,
							ISNULL(PTE.BillableHours,0) AS BillableHours,
							ISNULL(PTE.NonBillableHours,0) AS NonBillableHours,
							ISNULL(PFH.ProjectStatus,PTE.ProjectStatus) AS ProjectStatus
					FROM PersonForeCastedHours PFH
					FULL JOIN PersonTimeEntries PTE ON PTE.ProjectId = PFH.ProjectId AND PTE.PersonId = PFH.PersonId AND PTE.Timescale = PFH.Timescale
					INNER JOIN dbo.Timescale TS ON TS.TimescaleId = ISNULL(PFH.Timescale,PTE.Timescale)
					INNER JOIN dbo.Person P ON P.PersonId = ISNULL(PFH.PersonId,PTE.PersonId)
					WHERE (
							@IsUnassignedReport = 1 AND PFH.PersonId IS NULL AND PTE.BillableHours > 0.00 
						  ) 
						  OR 
						  (
							@IsUnassignedReport = 0 AND PTE.PersonId IS NULL AND PFH.ForecastedHours > 0.00
						  )
					ORDER BY P.LastName,P.FirstName
END
