CREATE PROCEDURE ProjectGroupDelete
	@GroupId		INT
AS

	SET NOCOUNT ON;
	DECLARE @Result INT,
			@Result_Success INT, @Result_GroupWasNotFound INT, @Result_GroupInUse INT
	SELECT	@Result_Success = 0,
			@Result_GroupWasNotFound = 1,
			@Result_GroupInUse = 2
		
	SET @Result = CASE
		WHEN NOT EXISTS(SELECT 1 FROM ProjectGroup WHERE GroupId = @GroupId)
			THEN @Result_GroupWasNotFound
		WHEN EXISTS(SELECT 1 FROM Project WHERE GroupId = @GroupId)
			THEN @Result_GroupInUse
		ELSE
			@Result_Success
		END
	
	IF @Result = @Result_Success
		DELETE ProjectGroup WHERE GroupId = @GroupId

	SELECT @Result Result

