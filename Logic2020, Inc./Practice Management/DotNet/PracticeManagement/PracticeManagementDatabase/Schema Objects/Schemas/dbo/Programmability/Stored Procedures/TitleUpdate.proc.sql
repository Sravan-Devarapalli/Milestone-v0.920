CREATE PROCEDURE [dbo].[TitleUpdate]
(
	@TitleId		INT,
	@Title			NVARCHAR(100),
	@TitleTypeId	INT,
	@SortOrder		INT,
	@PTOAccrual		INT,
	@MinimumSalary	INT,
	@MaximumSalary	INT,
	@UserLogin  NVARCHAR(255) 
)
AS
BEGIN
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	BEGIN TRY

	BEGIN TRAN TitleUpdate_Tran

		IF EXISTS(SELECT 1 FROM dbo.Title WHERE TitleId = @TitleId AND (ISNULL(MinimumSalary,0) <> ISNULL(@MinimumSalary,0) OR ISNULL(MaximumSalary,0) <> ISNULL(@MaximumSalary,0))) AND
		   EXISTS(SELECT 1 FROM Pay AS pa WHERE pa.TitleId = @TitleId AND pa.Timescale = 2 AND pa.SLTApproval = 1)--W2-Salary
		BEGIN

			UPDATE pa
			SET SLTApproval = 0
			FROM Pay AS pa 
			WHERE pa.TitleId = @TitleId 
					AND pa.Timescale = 2
					AND pa.SLTApproval = 1
		END

		UPDATE dbo.Title
		SET Title = @Title,
		TitleTypeId = @TitleTypeId,
		SortOrder = @SortOrder,
		PTOAccrual = @PTOAccrual,
		MinimumSalary = @MinimumSalary,
		MaximumSalary = @MaximumSalary
		WHERE TitleId = @TitleId

		COMMIT TRAN TitleUpdate_Tran

	END TRY
	BEGIN CATCH
		ROLLBACK TRAN TitleUpdate_Tran
		DECLARE	 @ERROR_STATE	tinyint
				,@ERROR_SEVERITY		tinyint
				,@ERROR_MESSAGE		    nvarchar(2000)
				,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)

	END CATCH

	EXEC SessionLogUnprepare 
END

