-- ==================================================
-- Author:		Sainathc
-- Create date: 05-14-2012
-- Description : Returns all the Approved by managers list
-- ==================================================
CREATE PROCEDURE [dbo].[GetApprovedByManagerList]
AS
BEGIN

	SELECT DISTINCT p.PersonId ,
			p.LastName ,
			p.FirstName 
	FROM dbo.Person AS p
	INNER JOIN dbo.aspnet_Users AS u ON p.Alias = u.UserName
	INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
	INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
	WHERE r.RoleName IN ('Administrator','Practice Area Manager','Business Unit Manager','Client Director','HR')
		AND p.PersonStatusId IN (1,5)
	ORDER BY p.LastName,p.FirstName 
END
