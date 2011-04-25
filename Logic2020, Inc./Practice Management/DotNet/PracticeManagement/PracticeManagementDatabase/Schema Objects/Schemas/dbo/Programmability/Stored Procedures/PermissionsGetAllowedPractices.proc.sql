CREATE PROCEDURE [dbo].[PermissionsGetAllowedPractices]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId FROM dbo.Permission WHERE TargetType = 5
END

