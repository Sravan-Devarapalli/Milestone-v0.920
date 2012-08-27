CREATE PROCEDURE [dbo].[GetPersonListWithRole]
(
	@RoleName NVARCHAR(100)
)
AS
BEGIN

	 SELECT p.PersonId ,
            p.FirstName ,
            p.LastName ,
            p.IsDefaultManager
    FROM    dbo.Person AS p
			INNER JOIN v_UsersInRoles AS ur ON ur.UserName = p.Alias AND LOWER(ur.RoleName) = LOWER(@RoleName)
    WHERE   p.PersonStatusId = 1 -- Active person only
    ORDER BY p.LastName ,p.FirstName
END

