CREATE PROCEDURE [dbo].[OwnerListAllShort]
	@PersonStatusId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT p.PersonId,
	       p.FirstName,
	       p.LastName
	FROM dbo.aspnet_Users AS u
	INNER JOIN dbo.Person AS p ON u.UserName = p.Alias
	INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
	INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
	WHERE (@PersonStatusId IS NULL OR p.PersonStatusId = @PersonStatusId)
		   AND r.RoleName IN ('Project lead' , 'Practice Area Manager' ,'Client Director')
	ORDER BY p.LastName, p.FirstName
END
