CREATE PROCEDURE dbo.PersonStatusHistoryUpdate
(
	@PersonId	INT,
	@PersonStatusId	INT
)
AS
BEGIN
	 DECLARE @Today DATETIME,
	 @TempDate DATETIME,
	 @TempDate2	DATETIME,
	 @IsStrawman BIT,
	 @PreviousStatusId INT 
	 
	 SET @Today  = CONVERT(DATE,[dbo].[GettingPMTime](GETDATE()))
	 SELECT @TempDate = CASE WHEN @PersonStatusId IN (1,3) THEN HireDate WHEN @PersonStatusId IN (2,5) THEN TerminationDate+1 END FROM dbo.Person WHERE PersonId = @PersonId
	 SELECT @IsStrawman = IsStrawman FROM dbo.Person WHERE PersonId = @PersonId
	 SELECT @PreviousStatusId = PersonStatusId FROM dbo.PersonStatusHistory WHERE PersonId = @PersonId AND EndDate IS NULL
	 IF @IsStrawman = 0
	 BEGIN
		 IF @PersonStatusId = 5
		 BEGIN
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
		 ELSE
		 BEGIN
				--If you set a status from one status to 'TerminationPending' today and now you are again setting status from 'TerminationPending' to the previous status on the same day.
				 IF EXISTS (SELECT 1 FROM dbo.PersonStatusHistory
									   WHERE EndDate = @Today-1
												AND PersonId = @PersonId
												AND PersonStatusId = @PersonStatusId
												AND @PreviousStatusId = 5)
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
				    IF EXISTS (SELECT 1 FROM dbo.PersonStatusHistory 
						WHERE EndDate IS NULL
								AND PersonId = @PersonId
								AND PersonStatusId <> @PersonStatusId)
					BEGIN
						UPDATE dbo.PersonStatusHistory
						SET EndDate = @TempDate-1
						WHERE EndDate IS NULL 
						AND  PersonId = @PersonId

						DELETE [dbo].[PersonStatusHistory]
						WHERE PersonId = @PersonId
						AND StartDate = @TempDate

						INSERT INTO [dbo].[PersonStatusHistory]
						([PersonId]
						,[PersonStatusId]
						,[StartDate]
						)
						VALUES(@PersonId,
						@PersonStatusId,
						@TempDate
						)

					END
					ELSE
					BEGIN
						IF EXISTS (SELECT 1 FROM PersonStatusHistory
								WHERE EndDate IS NULL 
										AND PersonId = @PersonId
					 					AND PersonStatusId = @PersonStatusId
										AND StartDate <> @TempDate)
						BEGIN
								SELECT @TempDate2 = StartDate
								FROM dbo.PersonStatusHistory
								WHERE EndDate IS NULL 
								AND PersonId = @PersonId

								DELETE [dbo].[PersonStatusHistory]
								WHERE PersonId = @PersonId
								AND StartDate = @TempDate

								UPDATE dbo.PersonStatusHistory
								SET StartDate = @TempDate
								WHERE StartDate = @TempDate2
								AND PersonId = @PersonId

								UPDATE dbo.PersonStatusHistory
								SET EndDate = @TempDate-1
								WHERE EndDate = @TempDate2-1
								AND PersonId = @PersonId
						END
					END
	
					DELETE dbo.PersonStatusHistory
					WHERE PersonId = @PersonId
							AND EndDate IS NOT NULL 
							AND StartDate >= @TempDate
					
					UPDATE dbo.PersonStatusHistory
					SET EndDate = @TempDate-1
					WHERE EndDate >= @TempDate
						  AND PersonId = @PersonId
						  AND EndDate IS NOT NULL 
				   
				END
		 END	
	 END
	 ELSE
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
					--When the strawman status is updated today with someother status
					IF EXISTS (SELECT 1 FROM dbo.PersonStatusHistory
							   WHERE StartDate = @Today
									 AND PersonId = @PersonId
									 AND PersonStatusId <> @PersonStatusId)
					BEGIN
						  UPDATE dbo.PersonStatusHistory
						  SET PersonStatusId = @PersonStatusId
						  WHERE StartDate = @Today
								AND PersonId = @PersonId
					END
					ELSE
					BEGIN
 						IF((SELECT COUNT(*) FROM dbo.PersonStatusHistory WHERE PersonId = @PersonId) > 0)
						BEGIN
							  UPDATE dbo.PersonStatusHistory
							  SET EndDate = @Today-1
							  WHERE PersonId = @PersonId
							  AND EndDate IS NULL
						END
						INSERT INTO [dbo].[PersonStatusHistory]
											([PersonId]
											,[PersonStatusId]
											,[StartDate]
											)
							   VALUES( @PersonId,
									   @PersonStatusId,
									   @Today
									 )
					END
			END
	 END
END

