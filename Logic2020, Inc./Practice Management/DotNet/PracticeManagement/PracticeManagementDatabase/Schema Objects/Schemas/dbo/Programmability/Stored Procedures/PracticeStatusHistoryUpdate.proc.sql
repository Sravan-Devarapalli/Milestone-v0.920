CREATE PROCEDURE dbo.PracticeStatusHistoryUpdate
(
	@PracticeId	INT,
	@IsActive	BIT
)
AS
BEGIN
	 DECLARE @Today DATETIME
	 SET @Today  = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))
	 IF NOT EXISTS (SELECT 1 FROM dbo.PracticeStatusHistory 
					WHERE EndDate IS NULL 
						AND  PracticeId = @PracticeId
						AND  IsActive = @IsActive)
	 BEGIN
		 -- Set the end date of the previous person status record to yester day
		 UPDATE dbo.PracticeStatusHistory
		 SET EndDate = @Today - 1
		 WHERE EndDate IS NULL 
				AND  PracticeId = @PracticeId
				AND  StartDate != @Today

		IF EXISTS (SELECT 1 FROM dbo.PracticeStatusHistory
					WHERE EndDate IS NULL 
								AND PracticeId = @PracticeId
								AND StartDate = @Today)
		BEGIN
			IF EXISTS (SELECT 1 FROM dbo.PracticeStatusHistory
					   WHERE EndDate = @Today-1
								AND PracticeId = @PracticeId
								AND IsActive = @IsActive)
			BEGIN
				UPDATE dbo.PracticeStatusHistory
				SET EndDate = NULL
				WHERE EndDate = @Today-1
					  AND PracticeId = @PracticeId
					  AND IsActive = @IsActive

				DELETE FROM  dbo.PracticeStatusHistory
				WHERE EndDate IS NULL 
					AND  PracticeId = @PracticeId
					AND  StartDate = @Today
			END
			ELSE
			BEGIN
				UPDATE dbo.PracticeStatusHistory
				SET IsActive = @IsActive
				WHERE EndDate IS NULL 
						AND  PracticeId = @PracticeId
						AND StartDate = @Today
			END
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].PracticeStatusHistory
			   ([PracticeId]
			   ,[IsActive]
			   ,[StartDate]
			   )
			VALUES
			   (@PracticeId
			   ,@IsActive
			   ,@Today
			   )
		END
	END
END
