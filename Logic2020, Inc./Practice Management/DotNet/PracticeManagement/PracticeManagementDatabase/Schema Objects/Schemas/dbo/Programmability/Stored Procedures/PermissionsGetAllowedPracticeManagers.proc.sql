CREATE PROCEDURE [dbo].[PermissionsGetAllowedPracticeManagers]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId FROM dbo.Permission WHERE TargetType = 4
END

