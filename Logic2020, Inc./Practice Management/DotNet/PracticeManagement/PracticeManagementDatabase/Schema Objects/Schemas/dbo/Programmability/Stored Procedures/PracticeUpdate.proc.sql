﻿-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-26
-- Description:	Practices routines
-- =============================================
CREATE PROCEDURE dbo.PracticeUpdate 
	@PracticeId INT,
	@Name VARCHAR(100),
	@PracticeManagerId INT,
	@IsActive BIT,
	@IsCompanyInternal BIT = 0	
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PreviousPracticeManager INT,
			@Today					 DATETIME,
			@IsNotesRequired		 BIT

	SELECT @PreviousPracticeManager = PracticeManagerId , @IsNotesRequired = IsNotesRequired
	FROM [dbo].[Practice]
	WHERE PracticeId = @PracticeId

	IF(@IsActive = 0)
	BEGIN
	SET @IsNotesRequired = 1
	END


	SELECT @Today = GETDATE()

	UPDATE [dbo].[Practice]
	   SET [Name] = @Name,
		   PracticeManagerId = @PracticeManagerId,
		   IsActive = @IsActive,
		   IsCompanyInternal = @IsCompanyInternal,
		   IsNotesRequired = @IsNotesRequired
	 WHERE PracticeId = @PracticeId
 
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

 END

