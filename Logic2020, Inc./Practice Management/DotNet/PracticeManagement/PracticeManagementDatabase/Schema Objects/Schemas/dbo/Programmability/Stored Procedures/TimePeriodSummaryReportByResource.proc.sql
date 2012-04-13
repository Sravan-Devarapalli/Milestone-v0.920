-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-12-2012
-- Description:  Time Entries grouped by Resource for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByResource]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal   DATETIME,
			@NOW			DATE,
			@HolidayTimeType INT

	SET @StartDateLocal = CONVERT(DATE,@StartDate)
	SET @EndDateLocal = CONVERT(DATE,@EndDate)
	SET @NOW = dbo.GettingPMTime(GETUTCDATE())
	SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()

	-- Get person level Default hours in between the StartDate and EndDate
	--1.Day should not be company holiday and also not converted to substitute day.
	--2.day should be company holiday and it should be taken as a substitute holiday.
	;WITH PersonDefaultHoursWithInPeriod
	AS
	(
	SELECT Pc.Personid,
		(COUNT(PC.Date) * 8) AS DefaultHours --Estimated working hours per day is 8.
	FROM (
			SELECT CAL.Date,
				   P.PersonId,
				   CAL.DayOff AS CompanyDayOff,
				   PCAL.TimeTypeId,
				   PCAL.SubstituteDate
			  FROM dbo.Calendar AS CAL
				   INNER JOIN dbo.Person AS P ON CAL.Date >= P.HireDate AND CAL.Date <= ISNULL(P.TerminationDate, dbo.GetFutureDate())
				   LEFT JOIN dbo.PersonCalendar AS PCAL ON PCAL.Date = CAL.Date AND PCAL.PersonId = P.PersonId
		) AS PC 
	WHERE PC.Date BETWEEN 
						@StartDate
						AND 
						CASE WHEN @EndDate > DATEADD(day,-1,@NOW) THEN DATEADD(day,-1,@NOW) ELSE  @EndDate END
			AND ( 
					(PC.CompanyDayOff = 0 AND ISNULL(PC.TimeTypeId, 0) != dbo.GetHolidayTimeTypeId()) 
					OR ( PC.CompanyDayOff =1 AND PC.SubstituteDate IS NOT NULL)
			)
	GROUP BY PC.PersonId 
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
	),AssignedPersons AS
	(
	  SELECT DISTINCT MP.PersonId
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
											AND MPE.StartDate < @EndDateLocal 
											AND @StartDateLocal  < MPE.EndDate
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId 
									AND p.IsStrawman = 0
									AND @StartDateLocal < ISNULL(P.TerminationDate,dbo.GetFutureDate()) 
	  INNER JOIN dbo.Project PRO ON PRO.ProjectId = M.ProjectId AND Pro.ProjectNumber != 'P031000'
	  INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = P.PersonId 
												AND PTSH.StartDate < @EndDateLocal 
												AND @StartDateLocal  < PTSH.EndDate
												AND PTSH.PersonStatusId = 1 --ACTIVE STATUS

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
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 AND CC.TimeEntrySectionId =1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS ProjectNonBillableHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 2 THEN TEH.ActualHours ELSE 0 END),2) AS BusinessDevelopmentHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 3 OR Pro.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS InternalHours,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 4 THEN TEH.ActualHours ELSE 0 END),2) AS AdminstrativeHours,
					ROUND (SUM(CASE WHEN ( CC.TimeEntrySectionId = 1 OR CC.TimeEntrySectionId = 2) AND @NOW > TE.ChargeCodeDate AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS ActualHours
			FROM dbo.TimeEntry TE
				INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
													AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal 
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
				INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
				INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
				INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = P.PersonId 
															AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND PTSH.EndDate
			WHERE  TE.ChargeCodeDate <= ISNULL(P.TerminationDate,dbo.GetFutureDate())
					AND (
							CC.timeTypeId != @HolidayTimeType
							OR (CC.timeTypeId = @HolidayTimeType AND PTSH.PersonStatusId = 1 )
						)	
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

