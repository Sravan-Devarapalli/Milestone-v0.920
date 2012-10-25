-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-26
-- Description:	Practices routines
-- =============================================
CREATE PROCEDURE dbo.PracticeInsert 
	@Name VARCHAR(100),
	@Abbreviation NVARCHAR(100) = NULL,
	@PracticeManagerId INT,
	@IsActive BIT = 1,
	@IsCompanyInternal BIT = 0 	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		BEGIN TRAN PracticeInsert_Tran;

		DECLARE @IsNotesRequired BIT = 1

		SELECT @IsNotesRequired =(SELECT  s.Value 
								FROM dbo.Settings AS s
								WHERE SettingsKey='NotesRequiredForTimeEntry' AND TypeId=4)

		IF(@IsActive = 0)
		BEGIN
		SET @IsNotesRequired = 1
		END
		DECLARE @Error NVARCHAR(200)
		IF EXISTS(SELECT 1 FROM dbo.Practice WHERE [Name] = @Name)
		BEGIN
			SET @Error = 'This Practice already exists. Please add a different Practice.'
			RAISERROR(@Error,16,1)
		END
		IF EXISTS(SELECT 1 FROM dbo.Practice WHERE ISNULL(Abbreviation,0) = @Abbreviation)
		BEGIN
			SET @Error = 'This practice area abbreviation already exists. Please add a different practice area abbreviation.'
			RAISERROR(@Error,16,1)
		END

		DECLARE @PracticeId INT

		INSERT INTO dbo.Practice (
			[Name],
			[Abbreviation],
			PracticeManagerId,
			IsActive,
			IsCompanyInternal,
			IsNotesRequired
		) VALUES ( 
			@Name,
			@Abbreviation,
			@PracticeManagerId,
			@IsActive,
			@IsCompanyInternal,
			@IsNotesRequired)

		SELECT @PracticeId = SCOPE_IDENTITY()

		INSERT INTO dbo.PracticeManagerHistory
						(PracticeId,		
						PracticeManagerId,	
						StartDate,			
						EndDate)
				VALUES
					(
					@PracticeId,
					@PracticeManagerId,
					dbo.InsertingTime(),
					NULL)
		EXEC dbo.PracticeStatusHistoryUpdate
			@PracticeId = @PracticeId,
			@IsActive = @IsActive

	COMMIT TRAN PracticeInsert_Tran;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN PracticeInsert_Tran;
		DECLARE	 @ERROR_STATE	tinyint
				,@ERROR_SEVERITY		tinyint
				,@ERROR_MESSAGE		    nvarchar(2000)
				,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH
END

