CREATE PROCEDURE [dbo].[SaveStrawman]
	@FirstName				NVARCHAR(100),
	@LastName				NVARCHAR(100),
	@UserLogin				NVARCHAR(255),
	@PersonId				INT OUTPUT,
	@Amount					DECIMAL(18,2),
	@Timescale				INT,
	@VacationDays			INT,
	@BonusAmount			DECIMAL(18,2),
	@BonusHoursToCollect	INT,
	@StartDate				DATETIME,
	@PersonStatusId         INT 
AS
BEGIN
	DECLARE @ErrorMessage NVARCHAR(2000)
	BEGIN TRY
		BEGIN TRAN TRAN_SaveStrawman

		EXEC dbo.SessionLogPrepare @userLogin = @UserLogin
		
		DECLARE @FutureDate  DATETIME, @HoursPerYear	DECIMAL
		SELECT @HoursPerYear = GHY.HoursPerYear FROM dbo.[BonusHoursPerYearTable]() GHY
		SELECT	@StartDate = ISNULL(CONVERT(DATE, @StartDate), '1900-01-01'),
				@BonusHoursToCollect = ISNULL(@BonusHoursToCollect, @HoursPerYear),
				@FutureDate = dbo.GetFutureDate()

		IF EXISTS (SELECT 1 FROM Person P
					WHERE (
							P.PersonId != @PersonId --edit strawman 
							OR @PersonId IS NULL --new strawman creating
						  ) 
						AND P.FirstName = @FirstName 
						AND P.LastName = @LastName
					)
			BEGIN
				-- Person First and Last Name uniqueness violation
				SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
				RAISERROR (@ErrorMessage, 16, 1)
			END

		IF @PersonId IS NULL
		BEGIN

			DECLARE @Counter INT,
					@StringCounter NVARCHAR(7),
					@EmployeeNumber NVARCHAR(12),
					@HireDate DATETIME
			SELECT @Counter = 0, @HireDate = dbo.GettingPMTime(GETUTCDATE())

			WHILE  (1 = 1)
			BEGIN

				SET @StringCounter = CONVERT(NVARCHAR(7), @Counter )
				IF LEN ( @StringCounter ) = 1
					SET @StringCounter =  '0' + @StringCounter

				SET @EmployeeNumber = 'C'+ SUBSTRING ( CONVERT(NVARCHAR(255), @HireDate, 10) ,0 , 3 ) +
					SUBSTRING ( CONVERT(NVARCHAR(255), @HireDate, 10) ,7 , 3 ) + @StringCounter
		
				IF (NOT EXISTS (SELECT 1 FROM [dbo].[Person] as p WHERE p.[EmployeeNumber] = @EmployeeNumber) )
					BREAK

				SET @Counter = @Counter + 1
			END
		
			INSERT INTO Person(FirstName,
							   LastName,
							   EmployeeNumber,
							   IsStrawman,
							   HireDate,
							   PersonStatusId)
			VALUES (@FirstName,
					@LastName,
					@EmployeeNumber,
					1,
					'1900-01-01',  --For strawman we will use HireDate field as created date field and it will be the pm minimum start date
					1)
		
			SET @PersonId = SCOPE_IDENTITY()
		END


		IF @PersonId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Person P WHERE P.IsStrawman = 1)
		BEGIN
			IF EXISTS (SELECT 1 
						FROM Person 
						WHERE PersonId = @PersonId 
							AND ( FirstName <> @FirstName OR LastName <> @LastName OR PersonStatusId <> @PersonStatusId) )
			BEGIN
				UPDATE Person
					SET FirstName = @FirstName, LastName = @LastName, PersonStatusId = @PersonStatusId
					WHERE PersonId = @PersonId

				EXEC dbo.PersonStatusHistoryUpdate
					@PersonId = @PersonId,
					@PersonStatusId = @PersonStatusId
			END
			
			IF NOT EXISTS (SELECT 1 FROM Pay P WHERE Person = @PersonId)
			BEGIN
				INSERT INTO Pay(Person,StartDate, EndDate, Amount, Timescale, VacationDays, BonusAmount, BonusHoursToCollect, IsActivePay)
				SELECT @PersonId, @StartDate , @FutureDate, @Amount, @Timescale, @VacationDays, @BonusAmount, @BonusHoursToCollect, 1
			END
			ELSE IF EXISTS (SELECT 1 FROM Pay pa 
						    WHERE pa.Person = @PersonId AND  pa.StartDate = @StartDate
							)
			BEGIN
				--IF Saving Second time in a day. then update previous saved on the same day. 
				--OR Updating existing compensation on that startdate.
				UPDATE pa
				Set pa.Amount = @Amount,
					pa.Timescale = @Timescale,
					pa.VacationDays = @VacationDays,
					pa.BonusAmount = @BonusAmount,
					Pa.BonusHoursToCollect = @BonusHoursToCollect
				FROM  Pay Pa
				WHERE Pa.Person = @PersonId AND pa.StartDate = @StartDate
			END
			ELSE --To update existing compensation enddate to this startdate and create new compensation from startdate to futuredate.
			BEGIN
				
				IF EXISTS (SELECT 1 FROM dbo.Pay Pa
							WHERE 
							Pa.EndDate = @FutureDate
							AND Pa.Person = @PersonId 
							AND
							(pa.Amount <> @Amount OR
							pa.Timescale <> @Timescale OR
							pa.VacationDays <> @VacationDays OR
							pa.BonusAmount <> @BonusAmount OR
							Pa.BonusHoursToCollect <> @BonusHoursToCollect) 
						 )
				BEGIN

						--End the Last compensation with Start date i.e. today.
						UPDATE Pay
						SET EndDate = @StartDate
						WHERE Person = @PersonId AND EndDate = @FutureDate

						--Insert new compensation with start date i.e. today and end with future date. 
						INSERT INTO Pay(Person,StartDate, EndDate, Amount, Timescale, VacationDays, BonusAmount, BonusHoursToCollect)
						SELECT @PersonId, @StartDate, @FutureDate, @Amount, @Timescale, @VacationDays, @BonusAmount, @BonusHoursToCollect

						UPDATE Pay	SET IsActivePay = CASE WHEN EndDate = @FutureDate   THEN 1 ELSE 0 END	WHERE Person = @PersonId
				END
			END
		END

		EXEC dbo.SessionLogUnprepare

		COMMIT TRAN TRAN_SaveStrawman

	END TRY
	BEGIN CATCH
		ROLLBACK TRAN TRAN_SaveStrawman
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END

