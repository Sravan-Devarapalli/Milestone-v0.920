-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-02-2012
-- Description:  Time Entries grouped by Resource for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByResource]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	DECLARE @NOW DATE
	SET @NOW = dbo.GettingPMTime(GETUTCDATE())

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
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
											AND ( MPE.StartDate BETWEEN @StartDate AND @EndDate
													OR MPE.EndDate BETWEEN @StartDate AND @EndDate )
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId AND p.IsStrawman = 0
	  GROUP BY MP.PersonId
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
			ISNULL(ROUND(CASE WHEN ISNULL(PDH.DefaultHours,0) = 0 THEN 0 ELSE (Data.ActualHours * 100 )/PDH.DefaultHours END,0),0) AS UtlizationPercent,
			CASE WHEN P.PersonStatusId = 2 THEN 'Terminated' ELSE TS.Name END AS Timescale
	FROM  
		(
			SELECT  TE.PersonId,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 AND CC.TimeEntrySectionId =1 THEN TEH.ActualHours ELSE 0 END),2) AS ProjectNonBillableHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 2 THEN TEH.ActualHours ELSE 0 END),2) AS BusinessDevelopmentHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 3 THEN TEH.ActualHours ELSE 0 END),2) AS InternalHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 4 THEN TEH.ActualHours ELSE 0 END),2) AS AdminstrativeHours,
					ROUND (SUM(CASE WHEN ( CC.TimeEntrySectionId = 1 OR CC.TimeEntrySectionId = 2) AND @NOW >= TE.ChargeCodeDate THEN TEH.ActualHours ELSE 0 END),2) AS ActualHours
					FROM dbo.TimeEntry TE
						INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
															AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
						INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
						INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
						INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					WHERE Pro.ProjectNumber != 'P031000' AND TE.ChargeCodeDate < ISNULL(P.TerminationDate,dbo.GetFutureDate())	
					GROUP BY TE.PersonId
		) Data
		FULL JOIN AssignedPersons AP ON AP.PersonId = Data.PersonId 
		INNER JOIN dbo.Person P ON (P.PersonId = Data.PersonId OR AP.PersonId = P.PersonId) AND p.IsStrawman = 0
		INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
		LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,dbo.GetFutureDate()) 
		LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
		LEFT JOIN PersonDefaultHoursWithInPeriod PDH ON PDH.PersonId = P.PersonId 
		ORDER BY P.LastName,P.FirstName
END

