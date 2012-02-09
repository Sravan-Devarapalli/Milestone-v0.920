CREATE PROCEDURE [dbo].[ListProjectsByClientAndPersonInPeriod]
(
	@ClientId INT,
	@IsOnlyActiveAndInternal	BIT = 0,-- = 1 If project status is active or internal
	@IsOnlyEnternalProjects    BIT = 0, -- =1 if project is external i.e. project.isinternal = 0
	@PersonId INT,
	@StartDate DATETIME,
	@EndDate DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	/*
		1	Inactive
		2	Projected
		3	Active
		4	Completed
		5	Experimental
		6	Internal
	*/
	;WITH UsedProjectIds
	AS 
	(
	  SELECT ProjectId
	  FROM dbo.PersonTimeEntryRecursiveSelection
	  WHERE PersonId = @PersonId 
		AND ClientId = @ClientId 
		AND StartDate <= @StartDate
		AND @EndDate <= ISNULL(EndDate,dbo.GetFutureDate())  
	)
	SELECT p.ProjectId,
			p.ProjectNumber +' - '+ p.Name as Name
	FROM dbo.v_Project AS p
	WHERE p.ClientId = @ClientId 
		AND p.IsAllowedToShow = 1 
		AND ((@IsOnlyEnternalProjects  = 1 AND p.IsInternal = 0) OR @IsOnlyEnternalProjects = 0 )
		AND (@IsOnlyActiveAndInternal = 1 AND p.ProjectStatusId IN (3,6))
		AND p.ProjectId NOT IN (SELECT ProjectId FROM UsedProjectIds)
	ORDER BY p.ProjectNumber

END
	

