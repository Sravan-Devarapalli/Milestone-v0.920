CREATE PROCEDURE [dbo].[SaveStrawManFromExisting]
	@FirstName				NVARCHAR(100),
	@LastName				NVARCHAR(100),
	@PersonId				INT OUTPUT,
	@Amount					DECIMAL(18,2),
	@Timescale				INT,
	@VacationDays			INT,
	@PersonStatusId         INT,
	@UserLogin				NVARCHAR(255)
AS
BEGIN
	DECLARE @ErrorMessage NVARCHAR(500)
	BEGIN TRY
		BEGIN TRAN TRAN_SaveStrawManFromExisting
		EXEC dbo.SessionLogPrepare @userLogin = @UserLogin

		DECLARE @ExistingPersonId INT,@Today DATETIME,@FutureDate  DATETIME
		SELECT  @ExistingPersonId = @PersonId,
				@Today = CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE())),
				@FutureDate = dbo.GetFutureDate()
	
		IF EXISTS (SELECT 1 FROM Person WHERE FirstName = @FirstName AND LastName = @LastName)
		BEGIN
			-- Person First and Last Name uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
			RAISERROR (@ErrorMessage, 16, 1)
		END
		

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

			SET @EmployeeNumber = 'C'+ SUBSTRING ( CONVERT(NVARCHAR(255), @HireDate, 10), 0, 3 ) +
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
				@PersonStatusId)
 		
		SET @PersonId = SCOPE_IDENTITY()

		/*
		1.Copy the complete pay history from existing person
		2.If last compensation record has startdate as today update the record
		3.else if check weather the amount/timescale/VacationDays are not same for the last record
			Then 
			a.End the Previous compensation upto  @Today.
			b.Insert new compensation with startdate today.
		4.Update the IsActivePay column in the pay table.

		*/

		IF NOT EXISTS (SELECT 1 FROM Pay P WHERE Person = @PersonId)
		BEGIN
			--1.Copy the complete pay history from existing person
			INSERT INTO Pay(Person,
							StartDate,
							EndDate,
							Amount,
							Timescale,
							VacationDays,
							BonusAmount,
							BonusHoursToCollect,
							PracticeId,
							IsActivePay)
			SELECT @PersonId,
					StartDate,
					EndDate,
					Amount,
					Timescale,
					VacationDays,
					BonusAmount,
					BonusHoursToCollect,
					PracticeId,
					IsActivePay 
			FROM dbo.Pay
     		WHERE Person = @ExistingPersonId

			--2.If last compensation record has startdate as today update the record
			IF EXISTS (SELECT 1 FROM Pay pa WHERE pa.Person = @PersonId AND  pa.StartDate = @Today)
			BEGIN
				UPDATE pa
				Set pa.Amount = @Amount,
				pa.Timescale = @Timescale,
				pa.VacationDays = @VacationDays
				FROM  Pay Pa
				WHERE Pa.Person = @PersonId AND pa.StartDate = @Today
			END

			--3.else if check weather the amount/timescale/VacationDays are not same for the last record
			ELSE IF EXISTS (SELECT 1 
							FROM dbo.Pay Pa
							WHERE Pa.Person = @PersonId
							AND Pa.EndDate = @FutureDate
							AND (pa.Amount <> @Amount OR
								pa.Timescale <> @Timescale OR
								pa.VacationDays <> @VacationDays)
						)
			BEGIN
					--a.End the Previous compensation upto  @Today.
					UPDATE Pay
					SET EndDate = @Today
					WHERE Person = @PersonId AND EndDate = @FutureDate

					--b.Insert new compensation with startdate today.
					INSERT INTO Pay(Person,StartDate, EndDate, Amount, Timescale, VacationDays, BonusAmount, BonusHoursToCollect)
					SELECT @PersonId, @Today, @FutureDate, @Amount, @Timescale, @VacationDays, BonusAmount, BonusHoursToCollect
					FROM  Pay Pa
					WHERE Pa.Person = @PersonId AND pa.EndDate = @Today
			END

			--4.Update the IsActivePay column in the pay table.
			UPDATE Pay
			SET IsActivePay = CASE WHEN EndDate = @FutureDate
									THEN 1 
									ELSE 0 
								END
			WHERE Person = @PersonId
		END
		
		EXEC dbo.SessionLogUnPrepare 

		COMMIT TRAN TRAN_SaveStrawManFromExisting
	END TRY
	BEGIN CATCH
		
		ROLLBACK TRAN TRAN_SaveStrawManFromExisting
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 16, 1)

	END CATCH
	
END

