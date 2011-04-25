CREATE PROCEDURE [dbo].[PermissionsGetAllowedClients]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId FROM dbo.Permission WHERE TargetType = 1
END

