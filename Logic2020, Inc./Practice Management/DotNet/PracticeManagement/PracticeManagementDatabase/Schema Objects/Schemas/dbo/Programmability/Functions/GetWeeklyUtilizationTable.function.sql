﻿CREATE FUNCTION [dbo].[GetWeeklyUtilizationTable]
(
	@StartDate DATETIME,
    @EndDate DATETIME,
	@Step INT = 7,
    @ActivePersons BIT = 1,
    @ActiveProjects BIT = 1,
    @ProjectedPersons BIT = 1,
    @ProjectedProjects BIT = 1,
    @ExperimentalProjects BIT = 1,
	@InternalProjects	BIT = 1,
	@TimescaleIds NVARCHAR(4000) = NULL,
	@PracticeIds NVARCHAR(4000) = NULL,
	@ExcludeInternalPractices BIT = 0
)
RETURNS TABLE 
AS
RETURN

/*
	1.Ranges:Gets the Ranges for the given step For the given dates( i.e. STARTDATE and ENDDATE).
	2.CurrentConsultantsWithRanges : Gets the consultants for the given filters ( i.e. @TimescaleIds,@PracticeIds,@ActivePersons,@ProjectedPersons,@ExcludeInternalPractices) mapped with Ranges.
	3.CurrentConsultantsWithVacationDays : Populate  VacationDays of the Consultants for the CurrentConsultantsWithRanges records.
	4.CurrentConsultantsWithProjectedHours : Populate  ProjectedHours of the Consultants for the CurrentConsultantsWithVacationDays records.
	5.CurrentConsultantsWithAvailableHours : Populate  AvailableHours of the Consultants for the CurrentConsultantsWithVacationDays records.
	6.CurrentConsultantsWithAllHours : Populate  all hours of the Consultants for the CurrentConsultantsWithVacationDays records.
	7.Final result : calculate the WeeklyUtlization with the obtained hours.

	Calculation of VacationDays: 

	Calculation of ProjectedHours:

	Calculation of AvailableHours:


*/

WIth Ranges
AS
(
	SELECT  c.MonthStartDate as StartDate,c.MonthEndDate  AS EndDate
	FROM dbo.Calendar c
	WHERE c.date BETWEEN @StartDate and @EndDate
	AND @Step = 30
	GROUP BY c.MonthStartDate,c.MonthEndDate  
	UNION ALL
	SELECT  c.date,c.date + 6
	FROM dbo.Calendar c
	WHERE c.date BETWEEN @StartDate and @EndDate
	AND DATEDIFF(day,@StartDate,c.date) % 7 = 0
	AND @Step = 7
	UNION ALL
	SELECT  c.date,c.date
	FROM dbo.Calendar c
	WHERE c.date BETWEEN @StartDate and @EndDate
	AND @Step = 1
),
CurrentConsultantsWithRanges
AS
(
	SELECT  P.PersonId,
			r.StartDate,
			CASE WHEN r.EndDate > @EndDate THEN @EndDate ELSE r.EndDate END EndDate,
			pay.Timescale,
			RANK() OVER (PARTITION BY P.PersonId,r.StartDate,r.EndDate  ORDER BY pay.startdate DESC) LatestPayRank
    FROM     Ranges r
	CROSS JOIN dbo.Person AS p
	INNER JOIN dbo.Timescale T ON T.TimescaleId IN ( SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@TimescaleIds))
	INNER JOIN dbo.[GetLatestPayWithInTheGivenRange](@StartDate,@EndDate) AS PCPT ON PCPT.PersonId = p.PersonId AND T.TimescaleId = PCPT.Timescale  
	LEFT JOIN dbo.Pay AS pay ON pay.Person = p.PersonId AND r.EndDate >= pay.StartDate AND r.StartDate <= (pay.EndDate-1)
    LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
    WHERE   (p.IsStrawman = 0)
            AND ( (@ActivePersons = 1 AND p.PersonStatusId IN (1,5)) -- active and termination pending statues
					OR
                    (@ProjectedPersons = 1 AND p.PersonStatusId = 3) -- projected status
				)
			AND (					
					p.DefaultPractice IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))
					AND (pr.IsCompanyInternal = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)
				)
),
CurrentConsultantsWithVacationDays
AS
(
	SELECT	CCWR.PersonId,
			CCWR.StartDate,
			CCWR.EndDate,
			CCWR.Timescale, 
			ISNULL(COUNT(PC.Date),0) AS VacationDays,
			ISNULL(SUM(CASE WHEN PC.CompanyDayOff = 0 THEN 1 ELSE 0 END),0) AS VacationDaysExcludingCompanyDayOff,
			ISNULL(SUM(CASE WHEN DATEPART(DW,PC.Date) <> 1 AND DATEPART(DW,PC.Date) <> 7 THEN 1 ELSE 0 END),0) AS VacationDaysIncludingCompanyDayOffsNotWeekends
	FROM CurrentConsultantsWithRanges CCWR
	LEFT JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = CCWR.PersonId 
										AND	PC.Date BETWEEN CCWR.StartDate AND CCWR.EndDate 
										AND (
												(
													PC.CompanyDayOff = 1 AND PC.DayOff = 1
												)
												OR
												(
													PC.DayOff = 1 AND PC.TimeOffHours = 8--if a person has added Timeoff  for complete 8 hr then the day is treated as vacation day.
												) 
											)
	WHERE CCWR.LatestPayRank = 1
	GROUP BY CCWR.PersonId,CCWR.StartDate,CCWR.EndDate,CCWR.Timescale
),
CurrentConsultantsWithProjectedHours
AS
(
 SELECT  S.PersonId,CONVERT(DECIMAL(10,2),SUM(S.HoursPerDay)) ProjectedHours,CCWV.StartDate ,CCWV.Timescale
    FROM CurrentConsultantsWithVacationDays CCWV
	INNER JOIN dbo.v_MilestonePersonSchedule AS S ON CCWV.PersonId = S.PersonId
    WHERE  s.MilestoneId <> (SELECT MilestoneId FROM dbo.DefaultMilestoneSetting)
			AND s.Date BETWEEN CCWV.startDate AND CCWV.endDate AND 
			(@ActiveProjects = 1 AND s.ProjectStatusId = 3 OR		--  3 - Active
			 @ProjectedProjects = 1 AND s.ProjectStatusId = 2 OR	--  2 - Projected
			 @ExperimentalProjects = 1 AND s.ProjectStatusId = 5 OR	--  5 - Experimental
			 @InternalProjects = 1 AND s.ProjectStatusId = 6) --6 - Internal
	GROUP BY s.PersonId ,CCWV.StartDate ,CCWV.Timescale
),
CurrentConsultantsWithAvailableHours
AS
(
--Salary Persons AvailableHours
	SELECT PC.PersonId ,CAST(SUM(8 - ISNULL(PC.TimeOffHours,0)) AS DECIMAL(10,2)) AS AvailableHours ,CCWV.StartDate 
	FROM CurrentConsultantsWithVacationDays CCWV
	INNER JOIN dbo.PersonCalendarAuto PC ON CCWV.PersonId = PC.PersonId 
										AND CCWV.Timescale = 2 
										AND PC.Date BETWEEN CCWV.StartDate AND CCWV.EndDate
										AND (PC.DayOff = 0 OR (PC.DayOff = 1 AND PC.CompanyDayOff = 0))
	GROUP BY PC.PersonId,CCWV.StartDate 
	UNION 
	SELECT CCWP.PersonId,CCWP.ProjectedHours AS AvailableHours ,CCWP.StartDate FROM  CurrentConsultantsWithProjectedHours CCWP WHERE CCWP.Timescale != 2 -- Non SalaryPerson AvailableHours
),
CurrentConsultantsWithAllHours
AS
(
	SELECT CCWV.PersonId,CCWV.StartDate,CCWV.EndDate,CCWV.VacationDays,CCWV.VacationDaysIncludingCompanyDayOffsNotWeekends,ISNULL(CCWA.AvailableHours,0) AS AvailableHours,ISNULL(CCWP.ProjectedHours,0) AS ProjectedHours,CCWV.Timescale
	FROM CurrentConsultantsWithVacationDays CCWV
	LEFT JOIN CurrentConsultantsWithAvailableHours CCWA ON CCWV.PersonId = CCWA.PersonId AND CCWV.StartDate = CCWA.StartDate
	LEFT JOIN CurrentConsultantsWithProjectedHours CCWP ON CCWV.PersonId = CCWP.PersonId AND CCWV.StartDate = CCWP.StartDate
)
SELECT	CC.PersonId,
		CC.StartDate,
		CC.EndDate,
		CC.AvailableHours,
		CC.ProjectedHours,
		CC.Timescale,
		CC.VacationDaysIncludingCompanyDayOffsNotWeekends AS VacationDays,
		CONVERT(INT,CASE	WHEN CC.VacationDays >= @Step
				THEN -1
				WHEN CC.AvailableHours = 0
				THEN 0
				ELSE CEILING(100*CC.ProjectedHours/CC.AvailableHours) 
				END) AS WeeklyUtlization
FROM CurrentConsultantsWithAllHours CC


