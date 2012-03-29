-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
-- Description:  Time Entries grouped by Resource for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByResource]
(
	@StartDate DATETIME,
	@EndDate   DATETIME,
	@SeniorityIds NVARCHAR(MAX) = NULL
)
AS
BEGIN

	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	DECLARE @NOW DATE
	SET @NOW = dbo.GettingPMTime(GETUTCDATE())
	DECLARE @SeniorityIdsTable TABLE(ID INT)
	INSERT INTO @SeniorityIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@SeniorityIds)

	-- Get person level Default hours in between the StartDate and EndDate
	--1.Day should not be company holiday and also not converted to substitute day.
	--2.day should be company holiday and it should be taken as a substitute holiday.
	;WITH PersonDefaultHoursWithInPeriod
	AS
	(
	SELECT Pc.Personid,
		(COUNT(PC.Date) * 8) AS DefaultHours --Estimated working hours per day is 8.
	FROM [dbo].[v_PersonCalendar] PC 
	WHERE PC.Date BETWEEN 
						@StartDate
						AND 
						CASE WHEN @EndDate > @NOW THEN @NOW ELSE  @EndDate END
			AND ( 
					(PC.CompanyDayOff = 0 AND ISNULL(PC.TimeTypeId, 0) != dbo.GetHolidayTimeTypeId()) 
					OR ( PC.CompanyDayOff =1 AND PC.SubstituteDate IS NOT NULL)
			)
	GROUP BY PC.PersonId 
	)
	,AssignedPersons AS
	(
	  SELECT MP.PersonId
			,MIN(CAST(M.IsHourlyAmount AS INT)) IsPersonNotAssignedToFixedProject
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
											AND ( MPE.StartDate BETWEEN @StartDate AND @EndDate
													OR MPE.EndDate BETWEEN @StartDate AND @EndDate )
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId AND p.IsStrawman = 0
	  GROUP BY MP.PersonId
	),
	DayBillratesByProjectsAndPerson AS
	(
	  SELECT MP.PersonId,
			M.ProjectId,
			C.Date,
			AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  WHERE C.Date BETWEEN @StartDate AND @EndDate 
	  GROUP BY MP.PersonId,
			M.ProjectId,
			C.Date
	)

	SELECT	P.PersonId,
			P.LastName,
			P.FirstName,
			S.SeniorityId,
			S.Name SeniorityName,
			ISNULL(Data.BillableHours,0) AS BillableHours,
			ISNULL(Data.ProjectNonBillableHours,0) AS ProjectNonBillableHours,
			ISNULL(Data.BusinessDevelopmentHours,0) AS BusinessDevelopmentHours,
			ISNULL(Data.InternalHours,0) AS InternalHours,
			ISNULL(Data.AdminstrativeHours,0) AS AdminstrativeHours,
			ISNULL(Data.BillableValue,0) AS BillableValue,
			ISNULL(ROUND(CASE WHEN ISNULL(PDH.DefaultHours,0) = 0 THEN 0 ELSE (Data.ActualHours * 100 )/PDH.DefaultHours END,0),0) AS UtlizationPercent,
			ISNULL(IsPersonNotAssignedToFixedProject,2) AS IsPersonNotAssignedToFixedProject,--if return 0 then fixed Amount else if return 1 not fixed Amount else if return 2 not fixed
			TS.Name AS Timescale
	FROM  
		(
			SELECT  TE.PersonId,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 AND CC.TimeEntrySectionId =1 THEN TEH.ActualHours ELSE 0 END),2) AS ProjectNonBillableHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 2 THEN TEH.ActualHours ELSE 0 END),2) AS BusinessDevelopmentHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 3 THEN TEH.ActualHours ELSE 0 END),2) AS InternalHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 4 THEN TEH.ActualHours ELSE 0 END),2) AS AdminstrativeHours,
					ROUND(SUM(ISNULL(DBPP.AvgBillRate,0) * (CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
																ELSE 0	
															END)
							),2) AS BillableValue,
					ROUND (SUM(CASE WHEN ( CC.TimeEntrySectionId = 1 OR CC.TimeEntrySectionId = 2) AND @NOW >= TE.ChargeCodeDate THEN TEH.ActualHours ELSE 0 END),2) AS ActualHours
					FROM dbo.TimeEntry TE
						INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
															AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
						INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
						INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
						LEFT JOIN DayBillratesByProjectsAndPerson DBPP ON DBPP.ProjectId = CC.ProjectId 
																		AND DBPP.PersonId = TE.PersonId
																		AND TE.ChargeCodeDate = DBPP.Date
					GROUP BY TE.PersonId
		) Data
		FULL JOIN AssignedPersons AP ON AP.PersonId = Data.PersonId 
		INNER JOIN dbo.Person P ON (P.PersonId = Data.PersonId OR AP.PersonId = P.PersonId) AND p.IsStrawman = 0
		INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
		LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate,dbo.GetFutureDate()) 
		LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
		LEFT JOIN PersonDefaultHoursWithInPeriod PDH ON PDH.PersonId = P.PersonId AND PA.Person = PDH.PersonId 
		WHERE (S.SeniorityId in (SELECT * FROM @SeniorityIdsTable) OR @SeniorityIds IS NULL)
		ORDER BY P.LastName,P.FirstName
END
