CREATE PROCEDURE dbo.GetResourceKeyValuePairsByResourceType
(
	@TypeId   INT
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT ResourcesKey,
           Value
	FROM dbo.StringResources
	WHERE TypeId=@TypeId
END
