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
	;WITH UsedProjectIds
		AS 
		(
		  SELECT ProjectId
		  FROM [dbo].[PersonTimeEntryRecursiveSelection]
		  WHERE PersonId = @PersonId 
			AND ProjectGroupId = @ProjectGroupId 
			AND StartDate <= @StartDate
			AND @EndDate <= ISNULL(EndDate,dbo.GetFutureDate())  
		)
	SELECT ProjectId,
		ProjectNumber +' - '+ Name as Name
	FROM dbo.Project 
	WHERE GroupId = @ProjectGroupId 
		AND IsInternal = @IsInternal 
		AND ProjectStatusId IN (3,6)
		AND ProjectId NOT IN (SELECT ProjectId FROM UsedProjectIds)
		AND ProjectNumber NOT IN ('P999905','P999906')
END
