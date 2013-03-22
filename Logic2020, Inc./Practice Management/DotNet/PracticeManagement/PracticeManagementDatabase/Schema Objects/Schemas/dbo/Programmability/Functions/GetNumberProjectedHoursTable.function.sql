CREATE FUNCTION [dbo].[GetNumberProjectedHoursTable]
(
      @startDate DATETIME,
      @endDate DATETIME,
   	  @ActiveProjects BIT = 1,
	  @ProjectedProjects BIT = 1,
	  @ExperimentalProjects BIT = 1,
	  @InternalProjects	BIT = 1
)
RETURNS TABLE 
AS

RETURN 
    SELECT  s.PersonId,SUM(s.HoursPerDay) ProjectedHours
    FROM    dbo.v_MilestonePersonSchedule AS s
    INNER JOIN dbo.Project AS pr ON pr.ProjectId = s.ProjectId
    WHERE  s.MilestoneId <> (SELECT MilestoneId FROM dbo.DefaultMilestoneSetting)
			AND s.Date BETWEEN @startDate AND @endDate AND 
			(@ActiveProjects = 1 AND pr.ProjectStatusId = 3 OR		--  3 - Active
			 @ProjectedProjects = 1 AND pr.ProjectStatusId = 2 OR	--  2 - Projected
			 @ExperimentalProjects = 1 AND pr.ProjectStatusId = 5 OR	--  5 - Experimental
			 @InternalProjects = 1 AND pr.ProjectStatusId = 6) --6 - Internal
	GROUP BY s.PersonId 

