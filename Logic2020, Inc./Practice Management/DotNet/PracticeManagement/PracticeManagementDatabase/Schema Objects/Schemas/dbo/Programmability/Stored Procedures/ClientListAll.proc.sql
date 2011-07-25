CREATE PROCEDURE dbo.ClientListAll
	@ShowAll BIT = 0,
	@PersonId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
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
		-- Populate is with the data from the v_Project 
		INSERT INTO @OwnerProjectClientList (ClientId) 
		SELECT proj.ClientId 
		FROM dbo.v_Project AS proj 
		WHERE proj.ProjectManagerId = @PersonId 
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

