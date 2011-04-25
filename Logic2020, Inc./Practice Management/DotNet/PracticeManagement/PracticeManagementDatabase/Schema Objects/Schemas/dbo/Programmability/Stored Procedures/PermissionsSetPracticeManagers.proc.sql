CREATE PROCEDURE [dbo].[PermissionsSetPracticeManagers]
	@PersonId INT,
	@TargetIdsList VARCHAR(500)
AS
	EXECUTE PermissionsSet
		@PersonId = @PersonId,
		@TargetType = 4,
		@TargetIdsList = @TargetIdsList

