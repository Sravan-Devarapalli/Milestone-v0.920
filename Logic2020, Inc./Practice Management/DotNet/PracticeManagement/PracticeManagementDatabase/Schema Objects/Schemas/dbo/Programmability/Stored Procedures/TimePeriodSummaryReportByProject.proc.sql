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
	@EndDate   DATETIME,
	@ClientIds NVARCHAR(MAX) = NULL,
	@PersonStatusIds NVARCHAR(MAX) = NULL
)
AS
BEGIN

	DECLARE @Today DATETIME
	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)
	SET @Today = dbo.GettingPMTime(GETUTCDATE())

	DECLARE @ClientIdsTable TABLE(ID INT)
	INSERT INTO @ClientIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@ClientIds)

	DECLARE @PersonStatusIdsTable TABLE(ID INT)
	INSERT INTO @PersonStatusIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PersonStatusIds)

	;WITH PersonProjectBillrates AS
	(
	  SELECT MP.PersonId,
			 M.ProjectId,
			 AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate,
			 C.Date
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId 
	  GROUP BY MP.PersonId,M.ProjectId,C.Date
	)
	,ProjectForeCastedHoursUntilToday AS
	(
	   SELECT M.ProjectId,
			 SUM(MPE.HoursPerDay) AS ForecastedHoursUntilToday,
			 MIN(CAST(M.IsHourlyAmount AS INT)) AS IsFixedProject --if return 0 then fixed project  else if return 1 Hourly project
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
			BillableValue,
			ISNULL(pfh.IsFixedProject,2) AS IsFixedProject, --if return 0 then fixed project else if return 1 Hourly project else if return 2 person not assigned to project.
			ISNULL(pfh.ForecastedHoursUntilToday,0) AS ForecastedHoursUntilToday,
			BillableHoursUntilToday,
			TimeEntrySectionId
	FROM(
			SELECT  CC.ClientId,
					CC.ProjectId,
					CC.ProjectGroupId,
					CC.TimeEntrySectionId,
					Round(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
					Round(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
					ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND TE.ChargeCodeDate <= @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday,
					ROUND(SUM(ISNULL(ppbr.AvgBillRate,0) * ( CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
																			ELSE 0	
																			END
																	)
						),2) AS BillableValue
			FROM dbo.TimeEntry TE
				INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND (CC.ClientId IN (SELECT * FROM @ClientIdsTable) OR @ClientIds IS NULL)
				INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId 
				LEFT JOIN PersonProjectBillrates ppbr ON ppbr.PersonId = P.PersonId AND TE.PersonId = ppbr.PersonId AND ppbr.ProjectId = CC.ProjectId AND ppbr.Date = TE.ChargeCodeDate
			GROUP BY CC.TimeEntrySectionId,
					 CC.ClientId,
					 CC.ProjectGroupId,
		 			 CC.ProjectId
		) Data
		INNER JOIN dbo.Project P ON P.ProjectId = Data.ProjectId
		INNER JOIN dbo.Client C ON C.ClientId = Data.ClientId 
		INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId AND (PS.ProjectStatusId IN (SELECT * FROM @PersonStatusIdsTable) OR @PersonStatusIds IS NULL)
		INNER JOIN dbo.ProjectGroup PG ON PG.ClientId = C.ClientId AND PG.GroupId = Data.ProjectGroupId
		LEFT JOIN  ProjectForeCastedHoursUntilToday pfh ON pfh.ProjectId = P.ProjectId
		ORDER BY TimeEntrySectionId,P.ProjectNumber
	
END
	
