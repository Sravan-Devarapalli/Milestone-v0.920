CREATE PROCEDURE ProjectGroupDelete
(
	@GroupId		INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
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
	BEGIN
	BEGIN TRY
		
		BEGIN TRAN TranProjectGroup
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		DELETE CTH
		FROM  dbo.ChargeCodeTurnOffHistory CTH
		INNER JOIN dbo.ChargeCode CC ON CC.Id = CTH.ChargeCodeId
		WHERE CC.ProjectGroupId = @GroupId 

		DELETE CC
		FROM  dbo.ChargeCode CC 
		WHERE cc.ProjectGroupId = @GroupId

		DELETE PTR
		FROM  dbo.PersonTimeEntryRecursiveSelection PTR 
		WHERE PTR.ProjectGroupId = @GroupId

		DELETE dbo.ProjectGroup 
		WHERE GroupId = @GroupId

		-- End logging session
		EXEC dbo.SessionLogUnprepare

        COMMIT TRAN TranProjectGroup

    END TRY
	BEGIN CATCH
	    
		ROLLBACK TRAN TranProjectGroup

		-- End logging session
		EXEC dbo.SessionLogUnprepare

		DECLARE @Error NVARCHAR(2000)
		SET @Error = ERROR_MESSAGE()
		RAISERROR(@Error, 16, 1)

	END CATCH

    END

	SELECT @Result Result
END
