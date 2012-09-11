CREATE PROCEDURE [dbo].[PersonListShortByRoleAndStatus]
	@PersonStatusIdsList NVARCHAR(50) = NULL,
	@RoleName		NVARCHAR(256) = NULL
AS
BEGIN
	DECLARE @PersonStatusIds TABLE(ID INT)
	INSERT INTO @PersonStatusIds (ID)
	SELECT ResultId 
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsList)

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
	WHERE (p.PersonStatusId IN (SELECT ResultId FROM @PersonStatusIds) OR @PersonStatusIdsList IS NULL)
			AND (ur.RoleName = @RoleName OR @RoleName IS NULL)
	ORDER BY LastName, FirstName
END
