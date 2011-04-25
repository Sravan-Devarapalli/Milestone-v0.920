CREATE PROCEDURE [dbo].[PermissionsGetAllowedGroups]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId FROM dbo.Permission WHERE TargetType = 2
END

