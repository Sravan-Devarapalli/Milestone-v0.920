-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-23
-- Description:	Shows report on milestones 
-- =============================================
CREATE PROCEDURE dbo.ConsultantMilestones 
	@PersonId INT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@IncludeActive BIT = 1,
	@includeInactive BIT = 1,
	@IncludeInternal BIT = 1,
	@includeExperimental BIT = 1,
	@includeProjected BIT = 1,
	@includeCompleted BIT = 1,
	@IncludeDefaultMileStone BIT = 1
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DefaultMilestoneId INT
	
	IF (@IncludeDefaultMileStone = 0)
	SELECT @DefaultMilestoneId = (SELECT TOP 1 MilestoneId
								  FROM dbo.DefaultMilestoneSetting
								  ORDER BY ModifiedDate DESC)
	;WITH ConsMiles AS (
	SELECT 
		cl.[Name] AS 'ClientName',
		cl.ClientId,
		pr.[Name] AS 'ProjectName',
		pr.ProjectId,
		pr.ProjectStatusId,
		pr.ProjectNumber,
		m.Description AS 'MilestoneName',
		m.MilestoneId,
		m.ConsultantsCanAdjust AS 'ConsultantsCanAdjust',
		m.IsChargeable,
		CASE 
			WHEN DATEDIFF(DAY, @StartDate, mpe.StartDate) >= 0 THEN mpe.StartDate
			ELSE @StartDate
		END AS 'PersonStartDate',
		mpe.MilestonePersonId,
		mpe.StartDate,
		CASE
			WHEN DATEDIFF(DAY, mpe.EndDate, @EndDate) >= 0 THEN mpe.EndDate
			ELSE @EndDate
		END AS 'PersonEndDate',
		mpe.EndDate,
		mpe.HoursPerDay,
		mpe.Amount		
	FROM dbo.Milestone AS m
	INNER JOIN dbo.MilestonePerson AS mp ON m.MilestoneId = mp.MilestoneId
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	INNER JOIN dbo.Project AS pr ON m.ProjectId = pr.ProjectId
	INNER JOIN dbo.Client AS cl ON pr.ClientId = cl.ClientId
	WHERE mp.PersonId = @PersonId AND 
		  (@IncludeDefaultMileStone = 1 OR m.MilestoneId <> @DefaultMilestoneId) AND 
		  (@IncludeDefaultMileStone = 1 OR m.IsDefault <> 1) AND
	      (mpe.StartDate BETWEEN @StartDate AND @EndDate 
			OR 
			mpe.EndDate BETWEEN @StartDate AND @EndDate
			OR 
			(DATEDIFF(DAY, mpe.StartDate, @StartDate) >= 0 AND DATEDIFF(DAY, @EndDate, mpe.EndDate) >= 0)
			OR
			(DATEDIFF(DAY, @StartDate, mpe.StartDate) >= 0 AND DATEDIFF(DAY, mpe.EndDate, @EndDate) >= 0)
		   )
		   AND
			(@IncludeActive = 1 OR (@includeInactive = 0 AND pr.ProjectStatusId <> 3))
		   AND
			(@includeInactive = 1 OR (@includeInactive = 0 AND pr.ProjectStatusId <> 1))
		   AND
			(@includeInternal = 1 OR (@includeInternal = 0 AND pr.ProjectStatusId <> 6))
			AND
			(@includeExperimental = 1 OR (@includeExperimental = 0 AND pr.ProjectStatusId <> 5))
		   AND
			(@includeCompleted = 1 OR (@includeCompleted = 0 AND pr.ProjectStatusId <> 4))
		   AND
			(@includeProjected = 1 OR (@includeProjected = 0 AND pr.ProjectStatusId <> 2))
		   
	)
	SELECT *
	FROM ConsMiles AS cm
	ORDER BY cm.StartDate DESC 
END

