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

	DECLARE @FutureDate DATETIME 
	SET @FutureDate = dbo.GetFutureDate()

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
		AND @EndDate <= ISNULL(EndDate,@FutureDate)  
	),
	AssignedProjects
	AS
	(
		SELECT DISTINCT P.ProjectId
		FROM dbo.Project P
		INNER JOIN dbo.Milestone M ON P.ProjectId = M.ProjectId 
								   AND P.ClientId = @ClientId 
		INNER JOIN dbo.MilestonePerson MP ON M.MilestoneId = MP.MilestoneId 
		INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId =MP.MilestonePersonId 
													AND (	@StartDate BETWEEN MPE.StartDate AND MPE.EndDate 
															OR @EndDate BETWEEN MPE.StartDate AND MPE.EndDate
															OR MPE.StartDate BETWEEN @StartDate AND @EndDate
														)
		INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId AND Per.PersonId = @PersonId
	)

	SELECT P.ProjectId,
			P.ProjectNumber +' - '+ P.Name as Name,
			CASE WHEN AP.ProjectId IS NULL THEN 0 ELSE 1 END AS [AssignedProject]
	FROM dbo.Project AS P
	INNER JOIN dbo.Client AS C ON P.ClientId = C.ClientId AND P.ClientId = @ClientId 
	LEFT JOIN AssignedProjects AP ON P.ProjectId = AP.ProjectId
	WHERE P.IsAllowedToShow = 1 
		AND ((@IsOnlyEnternalProjects  = 1 AND P.IsInternal = 0) OR @IsOnlyEnternalProjects = 0 )
		AND (@IsOnlyActiveAndInternal = 1 AND P.ProjectStatusId IN (3,6))
		AND P.ProjectId NOT IN (SELECT ProjectId FROM UsedProjectIds)
		AND ((@EndDate < '20120401') OR  ((@EndDate >= '20120401') AND P.ProjectId != 174 ))
		AND (ISNULL(P.InvisibleInTimeEntry,0) = 0)
	ORDER BY AssignedProject DESC ,P.ProjectNumber

END
	

