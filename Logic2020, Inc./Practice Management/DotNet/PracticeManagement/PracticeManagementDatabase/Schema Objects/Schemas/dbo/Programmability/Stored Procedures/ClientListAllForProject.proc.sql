-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-21-2008
-- Updated by:	
-- Update date:	
-- Description:	List clients available for the specific project
-- =============================================
CREATE PROCEDURE [dbo].[ClientListAllForProject]
(
	@ProjectId INT,
	@PersonId INT
)
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @IsAdmin BIT

	;WITH RequestersRoles 
	AS 
	(
		SELECT r.RoleName 
		FROM dbo.Person AS p
		INNER JOIN dbo.aspnet_Users AS u ON u.UserName = p.Alias
		INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
		INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
		WHERE p.PersonId = @PersonId 
	)

	SELECT @IsAdmin = 1
	FROM RequestersRoles AS roles 
	WHERE roles.RoleName = 'Administrator'  OR roles.RoleName = 'Salesperson'
		
	IF(@ProjectId IS NULL)
	BEGIN
		IF(@IsAdmin = 1)
		BEGIN
			SELECT c.ClientId, c.DefaultDiscount, c.DefaultTerms
			, c.DefaultSalespersonId, c.DefaultDirectorID, c.[Name], c.Inactive, c.IsChargeable
			FROM dbo.Client AS c 
			WHERE c.Inactive = 0
			ORDER BY c.[Name]
		END
		ELSE
		BEGIN
			SELECT c.ClientId, c.DefaultDiscount, c.DefaultTerms
			, c.DefaultSalespersonId, c.DefaultDirectorID, c.[Name], c.Inactive, c.IsChargeable
			FROM dbo.Client AS c
			JOIN dbo.Permission AS perm ON perm.PersonId = @PersonId
			WHERE (perm.TargetId = c.ClientId AND perm.TargetType = 1 AND c.Inactive = 0) 
				OR  (perm.TargetId IS NULL AND perm.TargetType = 1 AND c.Inactive = 0) 
			ORDER BY c.[Name]
		END
	END
	ELSE
	BEGIN
		IF(@IsAdmin = 1)
		BEGIN
			SELECT DISTINCT c.ClientId, c.DefaultDiscount, c.DefaultTerms
			, c.DefaultSalespersonId, c.DefaultDirectorID, c.[Name], c.Inactive, c.IsChargeable
			FROM dbo.Client AS c 
			JOIN dbo.Project    AS pro  ON  pro.ProjectId = @ProjectId 
			WHERE (c.Inactive = 0 )
				OR (pro.ClientId = c.ClientId )
			ORDER BY c.[Name]
		END
		ELSE
		BEGIN
			SELECT DISTINCT c.ClientId, c.DefaultDiscount, c.DefaultTerms
			, c.DefaultSalespersonId, c.DefaultDirectorID, c.[Name], c.Inactive, c.IsChargeable
			FROM dbo.Client AS c
			JOIN dbo.Permission AS perm ON  perm.PersonId = @PersonId
			JOIN dbo.Project    AS pro  ON  pro.ProjectId = @ProjectId 
			WHERE	(perm.TargetId = c.ClientId AND perm.TargetType = 1  AND c.Inactive = 0) 
				OR (perm.TargetId IS NULL AND perm.TargetType = 1   AND c.Inactive = 0)
				OR (pro.ClientId = c.ClientId )
			ORDER BY c.[Name]
		END			
	END		
END
