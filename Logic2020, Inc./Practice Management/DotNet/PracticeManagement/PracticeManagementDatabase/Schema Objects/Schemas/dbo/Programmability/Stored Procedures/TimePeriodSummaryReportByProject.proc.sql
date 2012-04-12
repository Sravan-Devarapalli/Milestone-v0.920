-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-12-2012
-- Description:  Time Entries grouped by Project for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByProject]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

	DECLARE @Today DATE,
			@StartDateLocal DATETIME,
			@EndDateLocal   DATETIME,
			@HolidayTimeType INT 
	SET @StartDateLocal = CONVERT(DATE,@StartDate)
	SET @EndDateLocal = CONVERT(DATE,@EndDate)
	SET @Today = dbo.GettingPMTime(GETUTCDATE())
	SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()

	;WITH ProjectForeCastedHoursUntilToday AS
	(
	   SELECT M.ProjectId,
			SUM(MPE.HoursPerDay) AS ForecastedHoursUntilToday,
			MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
			MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.v_PersonCalendar PC ON PC.PersonId = MP.PersonId 
											AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate
											AND PC.Date BETWEEN @StartDateLocal AND CASE WHEN @EndDateLocal > @Today THEN @Today ELSE  @EndDateLocal END
											AND ( 
													(PC.CompanyDayOff = 0 AND ISNULL(PC.TimeTypeId, 0) != dbo.GetHolidayTimeTypeId()) 
													OR ( PC.CompanyDayOff =1 AND PC.SubstituteDate IS NOT NULL)
												)
	  GROUP BY M.ProjectId 
	)
	,PersonTotalStatusHistory
	AS
	(
		SELECT PSH.PersonId,
				CASE WHEN PSH.StartDate = MinPayStartDate THEN PS.HireDate ELSE PSH.StartDate END AS StartDate,
				ISNULL(PSH.EndDate,dbo.GetFutureDate()) AS EndDate,
				PSH.PersonStatusId
		FROM dbo.PersonStatusHistory PSH 
		LEFT JOIN (
					SELECT PSH.PersonId
								,P.HireDate 
								,MIN(PSH.StartDate) AS MinPayStartDate
					FROM dbo.PersonStatusHistory PSH
					INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId
					WHERE P.IsStrawman = 0
					GROUP BY PSH.PersonId,P.HireDate
					HAVING P.HireDate < MIN(PSH.StartDate)
				) AS PS ON PS.PersonId = PSH.PersonId
		WHERE  PSH.StartDate < @EndDateLocal 
				AND @StartDateLocal  < ISNULL(PSH.EndDate,dbo.GetFutureDate())
	)

	SELECT  C.ClientId,
			C.Name AS ClientName,
			C.Code AS ClientCode,
			PG.Name AS GroupName,
			PG.Code AS GroupCode,
			P.ProjectId,
			P.Name AS ProjectName,
			P.ProjectNumber,
			PS.ProjectStatusId,
			PS.Name AS ProjectStatusName,
			BillableHours,
			NonBillableHours,
			ISNULL(pfh.ForecastedHoursUntilToday,0) AS ForecastedHoursUntilToday,
			BillableHoursUntilToday,
			TimeEntrySectionId,
			(CASE WHEN (pfh.MinimumValue IS NULL ) THEN '' 
			WHEN (pfh.MinimumValue = pfh.MaximumValue AND pfh.MinimumValue = 0) THEN 'Fixed'
			WHEN (pfh.MinimumValue = pfh.MaximumValue AND pfh.MinimumValue = 1) THEN 'Hourly'
			ELSE 'Both' END) AS BillingType
	FROM(
			SELECT  CC.ClientId,
					CC.ProjectId,
					CC.ProjectGroupId,
					CC.TimeEntrySectionId,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS [BillableHours],
			        ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR PRO.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS   [NonBillableHours],
				ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' AND TE.ChargeCodeDate < @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday
			FROM dbo.TimeEntry TE
				INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal 
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
				INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
				INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId 
				INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = P.PersonId 
															AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND PTSH.EndDate
				WHERE  TE.ChargeCodeDate <= ISNULL(P.TerminationDate,dbo.GetFutureDate())
					AND (
							CC.timeTypeId != @HolidayTimeType
							OR (CC.timeTypeId = @HolidayTimeType AND PTSH.PersonStatusId = 1 )
						)
			GROUP BY CC.TimeEntrySectionId,
					 CC.ClientId,
					 CC.ProjectGroupId,
		 			 CC.ProjectId
		) Data
		INNER JOIN dbo.Project P ON P.ProjectId = Data.ProjectId
		INNER JOIN dbo.Client C ON C.ClientId = Data.ClientId 
		INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId 
		INNER JOIN dbo.ProjectGroup PG ON PG.ClientId = C.ClientId AND PG.GroupId = Data.ProjectGroupId
		LEFT JOIN  ProjectForeCastedHoursUntilToday pfh ON pfh.ProjectId = P.ProjectId
		ORDER BY TimeEntrySectionId,P.ProjectNumber
	
END
	

