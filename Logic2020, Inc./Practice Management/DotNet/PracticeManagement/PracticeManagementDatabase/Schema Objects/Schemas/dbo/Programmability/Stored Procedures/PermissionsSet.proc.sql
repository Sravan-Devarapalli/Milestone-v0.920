CREATE PROCEDURE [dbo].[PermissionsSet]
	@PersonId INT,
	@TargetType INT,
	@TargetIdsList VARCHAR(500)
AS
	BEGIN TRANSACTION    -- Start the transaction

	-- Remove all previous permissions for the person
	--		of given permission target
	DELETE FROM [Permission]
	WHERE 
			PersonId = @PersonId 
		AND TargetType = @TargetType

	-- Insert new permissions
	IF @TargetIdsList IS NULL 
	INSERT 
		INTO [Permission] (PersonId, TargetType, TargetId)
		VALUES (@PersonId, @TargetType, NULL)
	ELSE 
	INSERT 
		INTO [Permission] (PersonId, TargetType, TargetId)
		SELECT @PersonId, @TargetType, tbl.ResultId 
		FROM ConvertStringListIntoTable(@TargetIdsList) AS tbl

	-- See if there is an error
	IF @@ERROR <> 0
	  -- There's an error b/c @ERROR is not 0, rollback
	  ROLLBACK
	ELSE
	  COMMIT   -- Success.  Commit the transaction

