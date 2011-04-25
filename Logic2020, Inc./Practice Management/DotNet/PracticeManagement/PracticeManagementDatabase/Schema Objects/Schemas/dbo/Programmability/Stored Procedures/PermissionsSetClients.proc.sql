CREATE PROCEDURE [dbo].[PermissionsSetClients]
	@PersonId INT,
	@TargetIdsList VARCHAR(500)
AS
	EXECUTE PermissionsSet
		@PersonId = @PersonId,
		@TargetType = 1,
		@TargetIdsList = @TargetIdsList

