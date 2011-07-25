-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-26
-- Description:	Practices routines
-- =============================================
CREATE PROCEDURE dbo.PracticeInsert 
	@Name VARCHAR(100),
	@PracticeManagerId INT,
	@IsActive BIT = 1,
	@IsCompanyInternal BIT = 0 	
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsNotesRequired BIT = 1

	SELECT @IsNotesRequired =(SELECT  s.Value 
							FROM dbo.Settings AS s
							WHERE SettingsKey='NotesRequiredForTimeEntry' AND TypeId=4)

	
	IF EXISTS(SELECT 1 FROM dbo.Practice WHERE [Name] = @Name)
	BEGIN
		DECLARE @Error NVARCHAR(200)
		SET @Error = 'This Practice already exists. Please add a different Practice.'
		RAISERROR(@Error,16,1)
		
	END
	DECLARE @PracticeId INT,
			@Today DATETIME
	 SET @Today  = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))

	INSERT INTO dbo.Practice (
		[Name],
		PracticeManagerId,
		IsActive,
		IsCompanyInternal,
		IsNotesRequired
	) VALUES ( 
		@Name,
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
				GETDATE(),
				NULL)
	EXEC dbo.PracticeStatusHistoryUpdate
		@PracticeId = @PracticeId,
		@IsActive = @IsActive
END

