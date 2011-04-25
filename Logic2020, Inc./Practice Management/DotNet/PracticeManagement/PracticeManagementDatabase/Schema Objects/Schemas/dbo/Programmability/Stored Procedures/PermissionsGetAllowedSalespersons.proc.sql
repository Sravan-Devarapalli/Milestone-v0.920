CREATE PROCEDURE [dbo].[PermissionsGetAllowedSalespersons]
(
	@PersonID INT
)
AS
BEGIN
	SELECT TargetId FROM dbo.Permission WHERE TargetType = 3
END

