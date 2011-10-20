CREATE PROCEDURE dbo.ClientListAll
	@ShowAll BIT = 0,
	@PersonId INT = NULL,
	@ApplyNewRule BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	/*
		Showing client of the Projects, Where the Logged in User is Project Manager OR Sales Person of 
		the Project(applicable only for Project Lead role Person) as part of #2941.
	*/

	DECLARE @UserHasHighRoleThanProjectLead INT = NULL
	--Adding Project Lead as per #2941.
	IF @PersonId IS NOT NULL AND EXISTS ( SELECT 1
				FROM aspnet_Users U
				JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
				JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
				JOIN Person P ON P.Alias = U.UserName
				WHERE P.PersonId = @PersonId AND R.LoweredRoleName = 'project lead')
	BEGIN
		SET @UserHasHighRoleThanProjectLead = 0
		
		SELECT @UserHasHighRoleThanProjectLead = COUNT(*)
		FROM aspnet_Users U
		JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
		JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
		JOIN Person P ON P.Alias = U.UserName
		WHERE P.PersonId = @PersonId
			AND R.LoweredRoleName IN ('administrator','client director','practice area manager','senior leadership')		
	END

	IF @UserHasHighRoleThanProjectLead = 0--Adding Project Lead as per #2941.
	BEGIN
		SET @ApplyNewRule = 1
	END

	IF @ApplyNewRule = 1	
	BEGIN

		--As per #2930, Not using Person Detail page Permissions.
		SELECT distinct C.ClientId
					, c.DefaultDiscount
					, c.DefaultTerms
					, c.DefaultSalespersonId
					, c.DefaultDirectorID
					, c.[Name]
					, c.Inactive
					, c.IsChargeable
		FROM Client C
		JOIN Project P ON P.ClientId = C.ClientId
		INNER JOIN dbo.ProjectManagers AS projmanager ON projmanager.ProjectId = P.ProjectId
		LEFT JOIN Commission Cm ON Cm.ProjectId = p.ProjectId AND Cm.CommissionType = 1
		WHERE ((@ShowAll = 0 AND C.Inactive = 0) OR @ShowAll <> 0)
		AND	(@PersonId IS null
			OR ( ISNULL(@UserHasHighRoleThanProjectLead,1) <> 0 AND p.DirectorId = @PersonId )--if Only Project Lead Role then we are not considering Director. as per #2941.
			OR Cm.PersonId = @PersonId
			OR projmanager.ProjectManagerId = @PersonId
			)
		ORDER BY C.Name

	END
	ELSE
	BEGIN

		-- Table variable to store list of Clients that owner is allowed to see	
		DECLARE @OwnerProjectClientList TABLE(
			ClientId INT NULL -- As per #2890
		)
	
		DECLARE @ClientPermissions TABLE(
			ClientId INT NULL
		)
		-- Populate is with the data from the permissions table
		--		TargetType = 1 means that we are looking for the clients in the permissions table
		INSERT INTO @ClientPermissions (
			ClientId
		) SELECT prm.TargetId FROM dbo.Permission AS prm WHERE prm.PersonId = @PersonId AND prm.TargetType = 1
	
		-- If there are nulls in permission table, it means that eveything is allowed to be seen,
		--		so set @PersonId to NULL which will extract all records from the table
		DECLARE @NullsNumber INT
		SELECT @NullsNumber = COUNT(*) FROM @ClientPermissions AS sp WHERE sp.ClientId IS NULL 	
		IF @NullsNumber > 0 
		BEGIN
			SET @PersonId = NULL
		END
		ELSE
		BEGIN
			INSERT INTO @OwnerProjectClientList (ClientId) 
			SELECT proj.ClientId 
			FROM dbo.Project AS proj
			INNER JOIN dbo.ProjectManagers AS projmanager ON projmanager.ProjectId = proj.ProjectId
			LEFT JOIN dbo.Commission C ON C.ProjectId = proj.ProjectId AND C.CommissionType = 1
			WHERE projmanager.ProjectManagerId = @PersonId OR C.PersonId = @PersonId -- Adding Salesperson - Project clients into the list.

		END

		IF @ShowAll = 0
		BEGIN
			SELECT 
					  c.ClientId
					, c.DefaultDiscount
					, c.DefaultTerms
					, c.DefaultSalespersonId
					, c.DefaultDirectorID
					, c.[Name]
					, c.Inactive
					, c.IsChargeable
				FROM Client AS c
				WHERE Inactive = 0
					AND (
						@PersonId IS NULL 
						OR ClientId IN (SELECT cp.ClientId FROM @ClientPermissions AS cp)
						OR ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc)
					)
				ORDER BY c.[Name]
		END
		ELSE
		BEGIN
			SELECT 
					  ClientId
					, DefaultDiscount
					, DefaultTerms
					, DefaultSalespersonId
					, DefaultDirectorID
					, [Name]
					, Inactive
					, IsChargeable
				FROM Client
				WHERE (
					@PersonId IS NULL 
					OR ClientId IN (SELECT cp.ClientId FROM @ClientPermissions AS cp)
					OR ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc)
				)
				ORDER BY [Name]
		END
	
	END
END

