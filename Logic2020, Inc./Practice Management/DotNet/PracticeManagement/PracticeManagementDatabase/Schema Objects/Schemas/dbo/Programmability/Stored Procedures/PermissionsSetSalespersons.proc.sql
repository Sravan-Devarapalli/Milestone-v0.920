CREATE PROCEDURE [dbo].[PermissionsSetSalespersons]
	@PersonId INT,
	@TargetIdsList VARCHAR(500)
AS
	EXECUTE PermissionsSet
		@PersonId = @PersonId,
		@TargetType = 3,
		@TargetIdsList = @TargetIdsList

