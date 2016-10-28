-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-29
-- Description:	Gets number of projected hours for the person
-- =============================================
CREATE FUNCTION dbo.GetNumberProjectedHours
    (
      @PersonId INT,
      @startDate DATETIME,
      @endDate DATETIME,
   	  @ActiveProjects BIT = 1,
	  @ProjectedProjects BIT = 1,
	  @ExperimentalProjects BIT = 1,
	  @ProposedProjects BIT =1,
	  @InternalProjects	BIT = 1,
	  @CompletedProjects BIT = 1,
	  @AtRiskProjects BIT =1
    )
RETURNS INT
AS BEGIN
    DECLARE @res INT ;

/*
----------- Project Status ------------
1	Inactive
2	Projected
3	Active
4	Completed
5	Experimental
*/

DECLARE @DefaultMilestoneId INT
	SELECT @DefaultMilestoneId = (SELECT TOP 1 MilestoneId
								  FROM dbo.DefaultMilestoneSetting
								  ORDER BY ModifiedDate DESC)

    SELECT  @res = SUM(s.HoursPerDay)
    FROM    dbo.v_MilestonePersonSchedule AS s
    INNER JOIN dbo.Project AS pr ON pr.ProjectId = s.ProjectId
    WHERE s.PersonId = @PersonId 
			AND s.MilestoneId <> @DefaultMilestoneId
			AND s.Date BETWEEN @startDate AND @endDate AND 
			(@ActiveProjects = 1 AND pr.ProjectStatusId = 3 OR		--  3 - Active
			 @ProjectedProjects = 1 AND pr.ProjectStatusId = 2 OR	--  2 - Projected
			 @ExperimentalProjects = 1 AND pr.ProjectStatusId = 5 OR	--  5 - Experimental
			 @ProposedProjects = 1 AND pr.ProjectStatusId = 7 OR --7-Proposed
			 @InternalProjects = 1 AND pr.ProjectStatusId = 6 OR--6 - Internal
			 @CompletedProjects = 1 AND pr.ProjectStatusId = 4 OR-- 4 - Completed
			 @AtRiskProjects = 1 ANd  pr.ProjectStatusId = 8)
    RETURN @res
   END

