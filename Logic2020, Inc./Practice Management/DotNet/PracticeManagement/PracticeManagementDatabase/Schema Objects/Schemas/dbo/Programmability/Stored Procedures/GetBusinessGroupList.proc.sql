CREATE PROCEDURE [dbo].[GetBusinessGroupList]
(
	@ClientId	INT = NULL,
	@BusinessUnitId INT = NULL
)
AS
BEGIN
	SELECT bg.BusinessGroupId,
			bg.ClientId,
			bg.Name ,
			bg.Code,
			dbo.[IsBusinessGroupInUse](bg.BusinessGroupId,bg.Code) AS InUse,
			bg.Active
	FROM dbo.BusinessGroup  bg
	LEFT JOIN dbo.ProjectGroup pg ON pg.BusinessGroupId = bg.BusinessGroupId
	WHERE  (@ClientId IS NULL OR bg.ClientId = @ClientId) AND (@BusinessUnitId IS NULL OR pg.GroupId = @BusinessUnitId)
END

