-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-26
-- Description:	Practices routines
-- =============================================
CREATE PROCEDURE dbo.PracticeUpdate 
	@PracticeId INT,
	@Name VARCHAR(100),
	@Abbreviation NVARCHAR(100) = NULL,
	@PracticeManagerId INT,
	@IsActive BIT,
	@IsCompanyInternal BIT = 0,
	@UserLogin NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC SessionLogPrepare @UserLogin = @UserLogin

	SELECT @Name = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@Name)),' ','<>'),'><',''),'<>',' '),
			   @Abbreviation = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@Abbreviation)),' ','<>'),'><',''),'<>',' ')

	DECLARE @PreviousPracticeManager INT,
			@Today					 DATETIME

	SELECT @PreviousPracticeManager = PracticeManagerId
	FROM [dbo].[Practice]
	WHERE PracticeId = @PracticeId

	SELECT @Today = GETDATE()

	UPDATE [dbo].[Practice]
	   SET [Name] = @Name,
		   [Abbreviation] = @Abbreviation,
		   PracticeManagerId = @PracticeManagerId,
		   IsActive = @IsActive,
		   IsCompanyInternal = @IsCompanyInternal
	 WHERE PracticeId = @PracticeId
			AND (
					[Name] != @Name
					OR ISNULL([Abbreviation],'') != ISNULL(@Abbreviation,'')
					OR PracticeManagerId != @PracticeManagerId
					OR IsActive != @IsActive
					OR IsCompanyInternal != @IsCompanyInternal
				)
 
	EXEC dbo.PracticeStatusHistoryUpdate
			@PracticeId	= @PracticeId,
			@IsActive =  @IsActive

	IF(@PracticeManagerId <> @PreviousPracticeManager)
	BEGIN
		
		UPDATE dbo.PracticeManagerHistory
		SET EndDate = @Today
		WHERE PracticeId = @PracticeId 
				AND PracticeManagerId = @PreviousPracticeManager
				AND EndDate IS NULL
		
		INSERT INTO dbo.PracticeManagerHistory(
					PracticeId,		
					PracticeManagerId,	
					StartDate,			
					EndDate)
		VALUES (@PracticeId,
				@PracticeManagerId,
				@Today,
				NULL)
	END

	EXEC dbo.SessionLogUnprepare
 END
