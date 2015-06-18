CREATE PROCEDURE [dbo].[UtilizationReport]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
BEGIN

	-- CALCULATION OF BILLABLE HOURS AND AVAILABLE HOURS COPIED FROM 'GetPersonTimeEntriesTotalsByPeriod' SPROC

	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal   DATETIME,
			@PersonIdLocal    INT,
			@HolidayTimeType INT,
			@NOW	DATE,
			@PersonTotalHours INT,
			@FutureDate DATETIME,
			@Yestaday DATE

	SELECT  @StartDateLocal = CONVERT(DATE,@StartDate),
			@EndDateLocal = CONVERT(DATE,@EndDate),
			@PersonIdLocal = @PersonId,
			@HolidayTimeType = dbo.GetHolidayTimeTypeId(),
			@NOW = dbo.GettingPMTime(GETUTCDATE()),
			@FutureDate = dbo.GetFutureDate(),
			@Yestaday = DATEADD(day,-1,@NOW)	

	-- Get person total hours in between the StartDate and EndDate
	--1.Day should not be company holiday and also not converted to substitute day.
	--2.day should be company holiday and it should be taken as a substitute holiday.
	
	
		SELECT @PersonTotalHours = (COUNT(PC.Date) * 8) --Estimated working hours per day is 8.
		FROM (
				SELECT PC.Date,
						PC.PersonId
					FROM PersonCalendarAuto PC
					INNER JOIN dbo.Pay pay ON pay.Person = PC.PersonId
						WHERE (PC.Date BETWEEN pay.StartDate AND pay.EndDate-1) AND pay.TimeScale = 2
			) AS PC 
		WHERE  PC.Date BETWEEN @StartDateLocal AND  @EndDateLocal AND PC.Date < @NOW
				AND DATENAME(weekday,PC.Date) != 'Saturday' AND DATENAME(weekday,PC.Date) != 'Sunday'
				AND PC.PersonId = @PersonIdLocal
		;WITH PersonByProjectsBillableTypes AS
		(
			SELECT P.PersonId,
				SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ProjectedHours
				--SUM(CASE WHEN PC.Date <= @Today THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay))
				--		ELSE 0
				--	END)  AS ProjectedHoursUntilToday
			FROM     dbo.MilestonePersonEntry AS MPE
			INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
			INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
										AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate
										AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal-1
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = M.ProjectId							
			WHERE MP.PersonId = @PersonIdLocal 
				AND PRO.ProjectStatusId IN (3,4,6) -- 3	Active, 4	Completed, 6 Internal
				AND PRO.Projectid <> 174
			GROUP BY P.PersonId
		)
		,
		BillableEntries
		AS
		(
				SELECT ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' THEN TEH.ActualHours 
								ELSE 0 
						END),2) AS BillableHours,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND TE.ChargeCodeDate < @NOW AND PRO.ProjectNumber != 'P031000' THEN TEH.ActualHours 
								ELSE 0 
						END),2) AS BillableHoursUntilToday,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR PRO.ProjectNumber = 'P031000' THEN TEH.ActualHours 
								ELSE 0 
						END),2) AS NonBillableHours,
						@PersonTotalHours AS AvailableHours,
						@PersonIdLocal AS PersonId
			FROM dbo.TimeEntry AS TE 
			INNER JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
			INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
			INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
			INNER JOIN dbo.PersonStatusHistory PTSH ON TE.ChargeCodeDate BETWEEN PTSH.StartDate  AND ISNULL(PTSH.EndDate,@FutureDate) AND PTSH.PersonId = TE.PersonId 
			INNER JOIN dbo.Pay pay ON pay.Person = TE.PersonId
			WHERE TE.PersonId = @PersonIdLocal 
				AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
				AND (TE.ChargeCodeDate BETWEEN pay.StartDate AND pay.EndDate-1) 
				AND pay.Timescale = 2
				AND (
						CC.timeTypeId != @HolidayTimeType
						OR (CC.timeTypeId = @HolidayTimeType AND PTSH.PersonStatusId IN (1,5) )
					)
				AND PRO.ProjectStatusId IN (3,4,6) --3  Active, 4	Completed, 6 Internal
		)

		SELECT	ISNULL(B.BillableHours,0) AS BillableHours,
				ISNULL(B.BillableHoursUntilToday,0) AS BillableHoursUntilToday,
				ISNULL(B.AvailableHours,0) AS AvailableHours,
				ISNULL(B.NonBillableHours,0) AS NonBillableHours,
				ROUND(CAST(ISNULL(P.ProjectedHours,0) AS REAL),2) AS ProjectedHours
		FROM PersonByProjectsBillableTypes P
		FULL JOIN BillableEntries B ON B.PersonId = P.PersonId
END

