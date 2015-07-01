CREATE PROCEDURE [dbo].[ProjectsListByProjectGroupId]
(
	@ProjectGroupId INT,
	@IsInternal		BIT,
	@PersonId INT,
	@StartDate DATETIME,
	@EndDate DATETIME
)
AS
BEGIN

	DECLARE @FutureDateLocal	DATETIME,
			@StartDateLocal		DATETIME,
			@EndDateLocal		DATETIME
			 
	SELECT @FutureDateLocal   = dbo.GetFutureDate(),
		   @StartDateLocal    = @StartDate,
		   @EndDateLocal      = @EndDate


	;WITH UsedProjectIds
		AS 
		(
		  SELECT PTR.ProjectId
		  FROM [dbo].[PersonTimeEntryRecursiveSelection] PTR
		  WHERE PTR.PersonId = @PersonId 
			AND PTR.ProjectGroupId = @ProjectGroupId 
			AND PTR.StartDate <= @StartDateLocal
			AND @EndDateLocal <= ISNULL(PTR.EndDate,@FutureDateLocal)  
		)
	SELECT P.ProjectId,
		   P.ProjectNumber +' - '+ P.Name as Name
	FROM dbo.Project AS P
	WHERE P.GroupId = @ProjectGroupId 
		AND P.IsInternal = @IsInternal 
		AND P.ProjectStatusId IN (3,6)
		AND P.ProjectId NOT IN (SELECT ProjectId FROM UsedProjectIds)
		AND P.IsAdministrative = 0
		AND (ISNULL(P.InvisibleInTimeEntry,0) = 0)
	ORDER BY P.ProjectNumber

END

