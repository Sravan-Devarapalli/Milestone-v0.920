CREATE PROCEDURE [dbo].[GetBusinessGroupList]
(
	@ClientIds	NVARCHAR(MAX) = NULL,
	@BusinessUnitId INT = NULL
)
AS
BEGIN

	DECLARE @ClientIdTable TABLE ( Ids INT )

	INSERT INTO @ClientIdTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ClientIds)
	
	SELECT DISTINCT bg.BusinessGroupId,
			bg.ClientId,
			C.Name AS ClientName,
			bg.Name ,
			bg.Code,
			dbo.[IsBusinessGroupInUse](bg.BusinessGroupId,bg.Code) AS InUse,
			bg.Active
	FROM dbo.BusinessGroup  bg
	INNER JOIN dbo.Client C ON C.ClientId = bg.ClientId
	LEFT JOIN dbo.ProjectGroup pg ON pg.BusinessGroupId = bg.BusinessGroupId
	WHERE  (@ClientIds IS NULL OR bg.ClientId IN (SELECT Ids FROM @ClientIdTable)) 
		AND (@BusinessUnitId IS NULL OR pg.GroupId = @BusinessUnitId) 
	ORDER BY bg.Name
END

