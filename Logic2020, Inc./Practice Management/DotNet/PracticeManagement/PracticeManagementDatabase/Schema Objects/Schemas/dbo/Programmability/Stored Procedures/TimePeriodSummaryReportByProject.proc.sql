-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
-- Description:  Time Entries grouped by Project for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByProject]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

	DECLARE @Today DATETIME
	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)
	SET @Today = dbo.GettingPMTime(GETUTCDATE())

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
											AND PC.Date BETWEEN @StartDate AND CASE WHEN @EndDate > @Today THEN @Today ELSE  @EndDate END
											AND ( 
													(PC.CompanyDayOff = 0 AND ISNULL(PC.TimeTypeId, 0) != dbo.GetHolidayTimeTypeId()) 
													OR ( PC.CompanyDayOff =1 AND PC.SubstituteDate IS NOT NULL)
												)
	  GROUP BY M.ProjectId 
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
					Round(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
					Round(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
					ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND TE.ChargeCodeDate <= @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday
			FROM dbo.TimeEntry TE
				INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
				INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId 
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
	
