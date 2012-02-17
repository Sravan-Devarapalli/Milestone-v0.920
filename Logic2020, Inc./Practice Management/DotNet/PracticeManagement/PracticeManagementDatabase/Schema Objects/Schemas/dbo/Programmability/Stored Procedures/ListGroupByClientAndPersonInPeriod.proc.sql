CREATE PROCEDURE [dbo].[ListGroupByClientAndPersonInPeriod]
	@ClientId	INT ,
	@PersonId	INT ,
	@StartDate	DATETIME,
	@ENDDATE	DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	;WITH UsedProjectGroupIds
	AS 
	(
	  SELECT ProjectGroupId
	  FROM dbo.PersonTimeEntryRecursiveSelection
	  WHERE PersonId = @PersonId 
		AND ClientId = @ClientId 
		AND StartDate <= @StartDate
		AND @EndDate <= ISNULL(EndDate,dbo.GetFutureDate())  
		AND ProjectId = (SELECT ProjectId FROM dbo.Project WHERE ProjectNumber = 'P999918')--Business Development Project

	)
	SELECT pg.GroupId
		, pg.Name
	FROM ProjectGroup AS pg
	INNER JOIN dbo.Client AS cl 
	ON pg.ClientId = cl.ClientId AND pg.ClientId = @ClientId  AND pg.Active = 1
	WHERE(pg.GroupId NOT IN (SELECT ProjectGroupId FROM UsedProjectGroupIds))
	ORDER BY pg.Name
END
