CREATE VIEW dbo.v_UsersInRoles
AS 
SELECT p.PersonId, p.LastName, p.FirstName, u.UserName, r.RoleName, p.PersonStatusId, st.Name as 'StatusName'
FROM dbo.aspnet_Users AS u
INNER JOIN dbo.Person AS p ON u.UserName = p.Alias
INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
inner join dbo.PersonStatus as st on st.PersonStatusId = p.PersonStatusId
--WHERE p.PersonStatusId = 1

