CREATE PROCEDURE [dbo].[PermissionsSetAll]
	@PersonId INT,
	@ClientIdsList VARCHAR(500),
	@GroupIdsList VARCHAR(500),
	@SalespersonIdsList VARCHAR(500),
	@PracticeManagerIdsList VARCHAR(500),
	@PracticeIdsList VARCHAR(500)
AS
	EXECUTE dbo.PermissionsSetClients
		@PersonId = @PersonId, -- INT
		@TargetIdsList = @ClientIdsList -- VARCHAR(500)
	
	EXECUTE dbo.PermissionsSetGroups
		@PersonId = @PersonId, -- INT
		@TargetIdsList = @GroupIdsList -- VARCHAR(500)
	
	EXECUTE dbo.PermissionsSetSalespersons
		@PersonId = @PersonId, -- INT
		@TargetIdsList = @SalespersonIdsList -- VARCHAR(500)
	
	EXECUTE dbo.PermissionsSetPracticeManagers
		@PersonId = @PersonId, -- INT
		@TargetIdsList = @PracticeManagerIdsList -- VARCHAR(500)
	
	EXECUTE dbo.PermissionsSetPractices
		@PersonId = @PersonId, -- INT
		@TargetIdsList = @PracticeIdsList -- VARCHAR(500)
		

