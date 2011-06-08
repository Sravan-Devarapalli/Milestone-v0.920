CREATE PROCEDURE dbo.PersonStatusHistoryUpdate
(
	@PersonId	INT,
	@PersonStatusId	INT
)
AS
BEGIN
	 DECLARE @Today DATETIME
	 SET @Today  = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))
	 
	 -- Set the end date of the previous person status record to yester day
	 IF NOT EXISTS (SELECT 1 FROM dbo.PersonStatusHistory 
					WHERE EndDate IS NULL 
						AND  PersonId = @PersonId
						AND  PersonStatusId = @PersonStatusId)
	 BEGIN
		 UPDATE dbo.PersonStatusHistory
		 SET EndDate = @Today-1
		 WHERE EndDate IS NULL 
				AND  PersonId = @PersonId
				AND StartDate != @Today

		IF EXISTS (SELECT 1 FROM dbo.PersonStatusHistory
					WHERE EndDate IS NULL 
								AND PersonId = @PersonId
								AND StartDate = @Today)
		BEGIN
			IF EXISTS (SELECT 1 FROM dbo.PersonStatusHistory
					   WHERE EndDate = @Today-1
								AND PersonId = @PersonId
								AND PersonStatusId = @PersonStatusId)
			BEGIN
				UPDATE dbo.PersonStatusHistory
				SET EndDate = NULL
				WHERE EndDate = @Today-1
					  AND PersonId = @PersonId
					  AND PersonStatusId = @PersonStatusId

				DELETE FROM  dbo.PersonStatusHistory
				WHERE EndDate IS NULL 
					AND  PersonId = @PersonId
					AND  StartDate = @Today
			END
			ELSE
			BEGIN
				UPDATE dbo.PersonStatusHistory
				SET PersonStatusId = @PersonStatusId
				WHERE EndDate IS NULL 
						AND  PersonId = @PersonId
						AND StartDate = @Today
			END
		END
		ELSE
		BEGIN	
			INSERT INTO [dbo].[PersonStatusHistory]
			   ([PersonId]
			   ,[PersonStatusId]
			   ,[StartDate]
			   )
			VALUES
			   (@PersonId
			   ,@PersonStatusId
			   ,@Today
			   )
		END
	END
END
