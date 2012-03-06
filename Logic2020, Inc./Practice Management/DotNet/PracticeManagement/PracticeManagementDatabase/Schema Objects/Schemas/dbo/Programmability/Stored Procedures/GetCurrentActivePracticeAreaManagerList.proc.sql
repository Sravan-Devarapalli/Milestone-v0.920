CREATE PROCEDURE [dbo].[GetCurrentActivePracticeAreaManagerList]
AS
BEGIN

	SELECT   p.PersonId ,
			p.LastName ,
			p.FirstName 
	FROM dbo.person AS p
	INNER JOIN dbo.aspnet_Users AS u ON p.Alias = u.UserName
	INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
	INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
	WHERE r.RoleName = 'Practice Area Manager' 
		AND p.PersonStatusId = 1
	ORDER BY 3,2

END
