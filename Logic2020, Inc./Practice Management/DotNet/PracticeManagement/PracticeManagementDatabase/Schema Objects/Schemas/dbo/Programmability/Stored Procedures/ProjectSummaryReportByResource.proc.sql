-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated by : Srinivas.M
-- Update Date: 09-25-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
	(
	  @ProjectNumber NVARCHAR(12) ,
	  @MilestoneId INT = NULL ,
	  @StartDate DATETIME = NULL ,
	  @EndDate DATETIME = NULL ,
	  @PersonRoleNames NVARCHAR(MAX) = NULL
	)
AS 
	BEGIN
		SET NOCOUNT ON;
		DECLARE @StartDateLocal DATETIME = NULL ,
			@EndDateLocal DATETIME = NULL ,
			@ProjectNumberLocal NVARCHAR(12) ,
			@MilestoneIdLocal INT = NULL ,
			@HolidayTimeType INT ,
			@ProjectId INT = NULL ,
			@Today DATE,
			@MilestoneStartDate DATETIME = NULL ,
			@MilestoneEndDate DATETIME = NULL,
			@FutureDate DATETIME

		SELECT @ProjectNumberLocal = @ProjectNumber,@FutureDate = dbo.GetFutureDate()
	
		SELECT  @ProjectId = P.ProjectId
		FROM    dbo.Project AS P
		WHERE   P.ProjectNumber = @ProjectNumberLocal
				AND @ProjectNumberLocal != 'P999918' --Business Development Project 

		IF ( @ProjectId IS NOT NULL ) 
			BEGIN

				SET @Today = dbo.GettingPMTime(GETUTCDATE())
				SET @MilestoneIdLocal = @MilestoneId
				SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()

				IF ( @StartDate IS NOT NULL
					 AND @EndDate IS NOT NULL
				   ) 
					BEGIN
						SET @StartDateLocal = CONVERT(DATE, @StartDate)
						SET @EndDateLocal = CONVERT(DATE, @EndDate)
					END

				IF ( @MilestoneIdLocal IS NOT NULL ) 
					BEGIN
						SELECT  @MilestoneStartDate = M.StartDate ,
								@MilestoneEndDate = M.ProjectedDeliveryDate
						FROM    dbo.Milestone AS M
						WHERE   M.MilestoneId = @MilestoneIdLocal 
					END

				DECLARE @PersonRoleNamesTable TABLE
					(
					  RoleName NVARCHAR(1024)
					)
				INSERT  INTO @PersonRoleNamesTable
						SELECT  ResultString
						FROM    [dbo].[ConvertXmlStringInToStringTable](@PersonRoleNames);
				WITH PersonForeCastedHoursRoleValues
					AS
					(	
						SELECT   MP.PersonId ,
								AVG(MPE.Amount) AS BillRate,
								PC.Date,
								SUM(CASE WHEN PC.Date <= @Today THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay))
									ELSE 0
								END) AS ForecastedHoursUntilToday,
								SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ForecastedHours,
								MAX(ISNULL(PR.RoleValue, 0)) AS MaxRoleValue ,
								MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
								MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue 
						FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
								INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
														AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate	
								LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
						WHERE    M.ProjectId = @ProjectId
								AND ( @MilestoneIdLocal IS NULL
										OR M.MilestoneId = @MilestoneIdLocal
									)
								AND ( ( @StartDateLocal IS NULL
										AND @EndDateLocal IS NULL
										)
										OR ( PC.Date BETWEEN @StartDateLocal AND @EndDateLocal )
									)
						GROUP BY MP.PersonId,PC.Date 
					),
					TimeEntryPersons 
					AS
					(
						SELECT TE.PersonId,TE.ChargeCodeDate,
						SUM(CASE WHEN ( TEH.IsChargeable = 1 AND @ProjectNumberLocal != 'P031000'
											AND TE.ChargeCodeDate <= @Today
													) THEN TEH.ActualHours
												ELSE 0
											END) BillableHoursUntilToday,
						SUM(CASE WHEN TEH.IsChargeable = 1  AND @ProjectNumberLocal != 'P031000'
												THEN TEH.ActualHours
												ELSE 0
											END) AS BillableHours ,
						SUM(CASE WHEN TEH.IsChargeable = 0 OR @ProjectNumberLocal = 'P031000'
												THEN TEH.ActualHours
												ELSE 0
											END) AS NonBillableHours
						--TE.*, TEH.IsChargeable, TEH.ActualHours, CC.*, PTSH.PersonStatusId
						FROM TimeEntry TE
						INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
						INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
																  AND ( ( @MilestoneIdLocal IS NULL )
																	OR ( TE.ChargeCodeDate BETWEEN @MilestoneStartDate AND @MilestoneEndDate )
																  )
																  AND ( ( @StartDateLocal IS NULL AND @EndDateLocal IS NULL )
																	OR ( TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal )
																  )
						INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
															  AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate)
						INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId 
													AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
													AND ( 
															CC.timeTypeId != @HolidayTimeType
															OR ( 
																	CC.timeTypeId = @HolidayTimeType
																	AND PTSH.PersonStatusId IN (1,5)
																)
														)
						GROUP BY TE.PersonId,TE.ChargeCodeDate
					),
					GroupedPersonDetails
					AS 
					(
						SELECT	ISNULL(PFR.PersonId,TEP.PersonId) AS PersonId,
								PFR.BillRate,
								SUM(PFR.ForecastedHoursUntilToday) AS ForecastedHoursUntilToday,
								SUM(PFR.ForecastedHours) AS ForecastedHours,
								MAX(ISNULL(PFR.MaxRoleValue, 0)) AS MaxRoleValue ,
								MIN(CAST(PFR.MinimumValue AS INT)) MinimumValue ,
								MAX(CAST(PFR.MaximumValue AS INT)) MaximumValue ,
								ROUND(ISNULL(SUM(TEP.BillableHoursUntilToday),0), 2) AS BillableHoursUntilToday ,
								ROUND(ISNULL(SUM(TEP.BillableHours),0), 2) AS BillableHours ,
								ROUND(ISNULL(SUM(TEP.NonBillableHours),0), 2) AS NonBillableHours
						FROM PersonForeCastedHoursRoleValues	PFR
						FULL JOIN TimeEntryPersons TEP ON TEP.PersonId = PFR.PersonId AND TEP.ChargeCodeDate = PFR.Date
						GROUP BY ISNULL(PFR.PersonId,TEP.PersonId),PFR.BillRate,CASE WHEN PFR.ForecastedHours IS NOT NULL THEN 0 ELSE NULL END
					)
					SELECT  P.PersonId ,
							P.LastName ,
							ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
							P.IsOffshore ,
							ISNULL(PR.Name, '') AS ProjectRoleName ,
							( 
								CASE	WHEN ( GPD.MinimumValue IS NULL ) THEN ''
										WHEN ( GPD.MinimumValue = GPD.MaximumValue AND GPD.MinimumValue = 0 ) THEN 'Fixed'
	 									WHEN ( GPD.MinimumValue = GPD.MaximumValue AND GPD.MinimumValue = 1) THEN 'Hourly'
								ELSE 'Both'
								END 
							) AS BillingType ,
							ROUND(MAX(ISNULL(GPD.ForecastedHoursUntilToday, 0)),2) AS ForecastedHoursUntilToday ,
							ROUND(MAX(ISNULL(GPD.ForecastedHours, 0)), 2) AS ForecastedHours,
							GPD.BillableHours,
							GPD.BillableHoursUntilToday,
							GPD.NonBillableHours,
							ISNULL(GPD.BillRate,0) AS BillRate,
							(CASE 
									WHEN ( (GPD.MinimumValue = GPD.MaximumValue AND GPD.MinimumValue = 1) 
											OR GPD.MinimumValue <> GPD.MaximumValue) 
									THEN ISNULL(GPD.BillRate,0) * GPD.BillableHours 
									ELSE 0 
							END) AS EstimatedBillings,
							P.EmployeeNumber
					FROM  dbo.Person P
					INNER JOIN GroupedPersonDetails AS GPD ON GPD.PersonId = P.PersonId
					LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = GPD.MaxRoleValue
					WHERE	(	@PersonRoleNames IS NULL
								OR ISNULL(PR.Name, '') IN ( SELECT RoleName FROM  @PersonRoleNamesTable)
							)
					GROUP BY P.PersonId ,
							P.LastName ,
							ISNULL(P.PreferredFirstName,P.FirstName) ,
							P.IsOffshore ,
							PR.Name ,
							GPD.MinimumValue ,
							GPD.MaximumValue,
							P.EmployeeNumber,
							GPD.BillRate,
							GPD.BillableHours,
							GPD.NonBillableHours,
							GPD.BillableHoursUntilToday
			END
		ELSE 
			BEGIN
				RAISERROR('There is no Project with this Project Number.', 16, 1)
			END
	END

