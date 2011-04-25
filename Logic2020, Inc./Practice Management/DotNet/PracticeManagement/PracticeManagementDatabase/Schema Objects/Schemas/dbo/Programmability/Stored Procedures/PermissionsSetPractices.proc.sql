CREATE PROCEDURE [dbo].[PermissionsSetPractices]
	@PersonId INT,
	@TargetIdsList VARCHAR(500)
AS
	EXECUTE PermissionsSet
		@PersonId = @PersonId,
		@TargetType = 5,
		@TargetIdsList = @TargetIdsList

