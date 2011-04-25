CREATE PROCEDURE [dbo].[PersonListShortByRoleAndStatus]
	@PersonStatusId	INT = NULL,
	@RoleName		NVARCHAR(256) = NULL
AS
BEGIN
	SELECT DISTINCT p.PersonId,
					p.FirstName,
					p.LastName,
					p.IsDefaultManager
	FROM dbo.Person p
	LEFT JOIN dbo.aspnet_Users u
	ON p.Alias = u.UserName
	LEFT JOIN dbo.aspnet_UsersInRoles uir
	ON u.UserId = uir.UserId
	LEFT JOIN dbo.aspnet_Roles ur
	ON ur.RoleId = uir.RoleId
	WHERE (p.PersonStatusId = @PersonStatusId OR @PersonStatusId IS NULL)
			AND (ur.RoleName = @RoleName OR @RoleName IS NULL)
	ORDER BY LastName, FirstName
END
