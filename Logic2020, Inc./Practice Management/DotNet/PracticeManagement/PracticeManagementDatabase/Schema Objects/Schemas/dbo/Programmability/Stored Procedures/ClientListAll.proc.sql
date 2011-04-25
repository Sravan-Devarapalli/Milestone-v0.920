CREATE PROCEDURE dbo.ClientListAll
	@ShowAll BIT = 0,
	@PersonId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @SalespersonPermissions TABLE(
		PersonId INT NULL
	)
	-- Populate is with the data from the permissions table
	--		TargetType = 1 means that we are looking for the clients in the permissions table
	INSERT INTO @SalespersonPermissions (
		PersonId
	) SELECT prm.TargetId FROM dbo.Permission AS prm WHERE prm.PersonId = @PersonId AND prm.TargetType = 1
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @SalespersonPermissions AS sp WHERE sp.PersonId IS NULL 	
	IF @NullsNumber > 0 SET @PersonId = NULL

	If @ShowAll = 0
	begin
		Select 
				c.ClientId
				, c.DefaultDiscount
				, c.DefaultTerms
				, c.DefaultSalespersonId
				, c.DefaultDirectorID
				, c.[Name]
				, c.Inactive
				, c.IsChargeable
			from Client AS c
			where Inactive = 0
				AND (
					@PersonId IS NULL 
					OR  
					ClientId IN (SELECT TargetId FROM dbo.Permission AS p WHERE p.PersonId = @PersonId AND p.TargetType = 1)
				)
			ORDER BY c.[Name]
	end
	else
	begin
		Select 
				ClientId
				, DefaultDiscount
				, DefaultTerms
				, DefaultSalespersonId
				, DefaultDirectorID
				, [Name]
				, Inactive
				, IsChargeable
			from Client
			WHERE (
				@PersonId IS NULL 
				OR  
				ClientId IN (SELECT TargetId FROM dbo.Permission AS p WHERE p.PersonId = @PersonId AND p.TargetType = 1)
			)
			ORDER BY [Name]
	end
END

