CREATE PROCEDURE [dbo].[OwnerListAllShort]
	@PersonStatusIds NVARCHAR(30) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PersonStatusIdsList TABLE(ID INT)
	INSERT INTO @PersonStatusIdsList (ID)
	SELECT ResultId 
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIds)

	SELECT DISTINCT p.PersonId,
	       p.FirstName,
	       p.LastName
	FROM dbo.aspnet_Users AS u
	INNER JOIN dbo.Person AS p ON u.UserName = p.Alias
	INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
	INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
	WHERE (@PersonStatusIds IS NULL OR p.PersonStatusId IN (SELECT ID FROM @PersonStatusIdsList))
		   AND r.RoleName IN ('Project lead' , 'Practice Area Manager' ,'Business Unit Manager','Client Director')
	ORDER BY p.LastName, p.FirstName
END	

