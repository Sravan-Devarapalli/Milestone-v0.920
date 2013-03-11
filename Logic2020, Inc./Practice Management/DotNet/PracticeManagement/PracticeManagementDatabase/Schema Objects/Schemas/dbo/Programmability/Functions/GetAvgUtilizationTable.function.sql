CREATE FUNCTION [dbo].[GetAvgUtilizationTable]
(
	@StartDate DATETIME,
	@EndDate DATETIME,
	@ActiveProjects BIT = 1,
	@ProjectedProjects BIT = 1,
	@ExperimentalProjects BIT = 1,
	@InternalProjects	BIT = 1  
)
RETURNS TABLE
/*
SalaryPersonsAvaliableHours:
Returns the AvaliableHours for all salary pay type( current pay type) persons.
If the person's current pay type is salary type
	returns the (no of working days in given period for the person * 8)
	i.e. no of working days in given period = total days in the period  - Person off days in the given period - company holidays in the given period

*/
AS
RETURN

	WITH SalaryPersonsAvaliableHours
	AS
	(
		SELECT PC.PersonId ,CAST(SUM(8 - ISNULL(PC.ActualHours,0)) AS DECIMAL(10,2)) AS AvaliableHours 
		FROM dbo.v_PersonCalendar PC
		INNER JOIN dbo.GetCurrentPayTypeTable() CPT ON CPT.PersonId = PC.PersonId AND CPT.Timescale = 2
		WHERE PC.Date BETWEEN @StartDate AND @EndDate 
				AND (PC.DayOff = 0 OR (PC.DayOff = 1 AND PC.CompanyDayOff = 0))
		GROUP BY PC.PersonId
	),
	SalaryPersonProjectedHours
	AS
	(
	SELECT  p.PersonId,CAST (ISNULL(SUM(MPS.HoursPerDay),0) AS DECIMAL(10,2)) AS ProjectedHours 
    FROM    dbo.Person P 
    INNER JOIN dbo.GetCurrentPayTypeTable() CPT ON CPT.PersonId = P.PersonId AND CPT.Timescale = 2
	LEFT JOIN dbo.v_MilestonePersonSchedule AS MPS ON P.PersonId = MPS.PersonId 
												AND MPS.MilestoneId <> (SELECT MilestoneId FROM dbo.DefaultMilestoneSetting)
												AND MPS.IsDefaultMileStone = 0
												AND MPS.Date BETWEEN @StartDate AND @EndDate
												AND (
													 @ActiveProjects = 1 AND MPS.ProjectStatusId = 3 OR		--  3 - Active
													 @ProjectedProjects = 1 AND MPS.ProjectStatusId = 2 OR	--  2 - Projected
													 @ExperimentalProjects = 1 AND MPS.ProjectStatusId = 5 OR	--  5 - Experimental
													 @InternalProjects = 1 AND MPS.ProjectStatusId = 6 --6 - Internal
													)
	GROUP BY P.PersonId
	)

	SELECT SP.PersonId,
	CONVERT(INT,CASE WHEN ISNULL(SPAH.AvaliableHours ,0) = 0 THEN 0 
		 ELSE CEILING(100*SP.ProjectedHours/SPAH.AvaliableHours)
	END) AS AvgUtilization,SP.ProjectedHours,SPAH.AvaliableHours
	FROM SalaryPersonProjectedHours SP 
	LEFT JOIN SalaryPersonsAvaliableHours SPAH ON SP.PersonId = SPAH.PersonId

	UNION 
	SELECT P.PersonId,100  AS AvgUtilization,SP.ProjectedHours,Sp.PersonId
	FROM dbo.Person P 
	LEFT JOIN SalaryPersonProjectedHours SP ON P.PersonId = SP.PersonId
	WHERE SP.PersonId IS NULL -- NON Salary Persons Will always have avg utlization as 100

