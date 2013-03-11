CREATE FUNCTION [dbo].[GetWeeklyUtilizationTable]
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
	5.CurrentConsultantsWithAvaliableHours : Populate  AvaliableHours of the Consultants for the CurrentConsultantsWithVacationDays records.
	6.CurrentConsultantsWithAllHours : Populate  all hours of the Consultants for the CurrentConsultantsWithVacationDays records.
	7.Final result : calculate the WeeklyUtlization with the obtained hours.

	Calculation of VacationDays: 

	Calculation of ProjectedHours:

	Calculation of AvaliableHours:


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
			PCPT.Timescale
    FROM     Ranges r
	CROSS JOIN dbo.Person AS p
	INNER JOIN dbo.Timescale T ON T.TimescaleId IN ( SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@TimescaleIds))
	INNER JOIN dbo.GetCurrentPayTypeTable() AS PCPT ON PCPT.PersonId = p.PersonId AND T.TimescaleId = PCPT.Timescale
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
			ISNULL(COUNT(PC.Date),0) AS VacationDays
	FROM CurrentConsultantsWithRanges CCWR
	LEFT JOIN dbo.v_PersonCalendar PC ON PC.PersonId = CCWR.PersonId 
										AND	PC.Date BETWEEN CCWR.StartDate AND CCWR.EndDate 
										AND (PC.DayOff = 1 AND PC.CompanyDayOff = 1)
	GROUP BY CCWR.PersonId,CCWR.StartDate,CCWR.EndDate,CCWR.Timescale
),
CurrentConsultantsWithProjectedHours
AS
(
 SELECT  S.PersonId,CONVERT(DECIMAL(10,2),SUM(S.HoursPerDay)) ProjectedHours,CCWV.StartDate ,CCWV.Timescale
    FROM CurrentConsultantsWithVacationDays CCWV
	INNER JOIN dbo.v_MilestonePersonSchedule AS S ON CCWV.PersonId = S.PersonId
    WHERE  s.MilestoneId <> (SELECT MilestoneId FROM dbo.DefaultMilestoneSetting)
			AND s.IsDefaultMileStone = 0
			AND s.Date BETWEEN CCWV.startDate AND CCWV.endDate AND 
			(@ActiveProjects = 1 AND s.ProjectStatusId = 3 OR		--  3 - Active
			 @ProjectedProjects = 1 AND s.ProjectStatusId = 2 OR	--  2 - Projected
			 @ExperimentalProjects = 1 AND s.ProjectStatusId = 5 OR	--  5 - Experimental
			 @InternalProjects = 1 AND s.ProjectStatusId = 6) --6 - Internal
	GROUP BY s.PersonId ,CCWV.StartDate ,CCWV.Timescale
),
CurrentConsultantsWithAvaliableHours
AS
(
--Salary Persons AvaliableHours
	SELECT PC.PersonId ,CAST(SUM(8 - ISNULL(PC.ActualHours,0)) AS DECIMAL(10,2)) AS AvaliableHours ,CCWV.StartDate 
	FROM CurrentConsultantsWithVacationDays CCWV
	INNER JOIN dbo.v_PersonCalendar PC ON CCWV.PersonId = PC.PersonId 
										AND CCWV.Timescale = 2 
										AND PC.Date BETWEEN CCWV.StartDate AND CCWV.EndDate
										AND (PC.DayOff = 0 OR (PC.DayOff = 1 AND PC.CompanyDayOff = 0))
	GROUP BY PC.PersonId,CCWV.StartDate 
	UNION 
	SELECT CCWP.PersonId,CCWP.ProjectedHours AS AvaliableHours ,CCWP.StartDate FROM  CurrentConsultantsWithProjectedHours CCWP WHERE CCWP.Timescale != 2 -- Non SalaryPerson AvaliableHours
),
CurrentConsultantsWithAllHours
AS
(
	SELECT CCWV.PersonId,CCWV.StartDate,CCWV.EndDate,CCWV.VacationDays,ISNULL(CCWA.AvaliableHours,0) AS AvaliableHours,ISNULL(CCWP.ProjectedHours,0) AS ProjectedHours
	FROM CurrentConsultantsWithVacationDays CCWV
	LEFT JOIN CurrentConsultantsWithAvaliableHours CCWA ON CCWV.PersonId = CCWA.PersonId AND CCWV.StartDate = CCWA.StartDate
	LEFT JOIN CurrentConsultantsWithProjectedHours CCWP ON CCWV.PersonId = CCWP.PersonId AND CCWV.StartDate = CCWP.StartDate
)
SELECT	CC.PersonId,
		CC.StartDate,
		CC.EndDate,
		CC.AvaliableHours,
		CC.ProjectedHours,
		CONVERT(INT,CASE	WHEN CC.VacationDays >= @Step
				THEN -1
				WHEN (
					(@Step = 1 AND DATEPART(dw, CC.StartDate)IN(7,1)) 
						OR
					(CC.AvaliableHours = 0)
					)
				THEN 0
				ELSE CEILING(100*CC.ProjectedHours/CC.AvaliableHours) 
				END) AS WeeklyUtlization
FROM CurrentConsultantsWithAllHours CC


