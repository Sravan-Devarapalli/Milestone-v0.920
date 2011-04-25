CREATE PROCEDURE [dbo].[PermissionsGetAll]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId, TargetType FROM dbo.Permission WHERE PersonID = @PersonID
END

