CREATE FUNCTION [dbo].[GetNumberAvaliableHoursTable]
  (
      @startDate DATETIME,
      @endDate DATETIME,
   	  @ActiveProjects BIT = 1,
	  @ProjectedProjects BIT = 1,
	  @ExperimentalProjects BIT = 1	,
	  @InternalProjects		BIT = 1
    )
/*
If the person's current pay type is salary type
	returns the (no of working days in given period for the person * 8)
	i.e. no of working days in given period = total days in the period  - Person off days in the given period - company holidays in the given period
else
returns Projected hours in the given period for the given Project status types(ACTIVE/PROJECTED/INTERNAL/EXPERIMENTAL)

Calculation of projected hours:
1.Sum's up the person assigned hours to the milestones in the:
a.For the given time period.
b.For the given project status types.
c.Excludes the company holidays,sun,sat. 
d.Also excludes the default milestone project.

*/
RETURNS TABLE
AS 
RETURN

   WITH SalaryPersonsAvaliableHours
	AS
	(
		SELECT PC.PersonId ,(COUNT(*) * 8) AS AvaliableHours 
		FROM dbo.v_PersonCalendar PC
		INNER JOIN dbo.GetCurrentPayTypeTable() CPT ON CPT.PersonId = PC.PersonId AND CPT.Timescale = 2
		WHERE PC.Date BETWEEN @StartDate AND @EndDate 
				AND PC.DayOff = 0
		GROUP BY PC.PersonId
	),
	NonSalaryPersonAvaliableHours
	AS
	(
	SELECT  p.PersonId,CAST (ISNULL(SUM(MPS.HoursPerDay),0) AS INT) AS AvaliableHours 
    FROM    dbo.Person P 
    INNER JOIN dbo.GetCurrentPayTypeTable() CPT ON CPT.PersonId = P.PersonId AND CPT.Timescale != 2
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
    SELECT SPAH.PersonId,SPAH.AvaliableHours FROM SalaryPersonsAvaliableHours SPAH 
	UNION 
	SELECT NSPAH.PersonId,NSPAH.AvaliableHours FROM NonSalaryPersonAvaliableHours NSPAH 

