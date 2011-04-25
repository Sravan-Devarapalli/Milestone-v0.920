CREATE PROCEDURE [dbo].[PermissionsSetGroups]
	@PersonId INT,
	@TargetIdsList VARCHAR(500)
AS
	EXECUTE PermissionsSet
		@PersonId = @PersonId,
		@TargetType = 2,
		@TargetIdsList = @TargetIdsList

