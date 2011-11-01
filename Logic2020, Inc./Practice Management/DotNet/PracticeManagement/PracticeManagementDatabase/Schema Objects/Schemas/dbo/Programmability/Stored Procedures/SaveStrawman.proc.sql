CREATE PROCEDURE [dbo].[SaveStrawman]
	@FirstName				NVARCHAR(100),
	@LastName				NVARCHAR(100),
	@UserLogin				NVARCHAR(255),
	@PersonId				INT OUTPUT,
	@Amount					DECIMAL(18,2),
	@Timescale				INT,
	@TimesPaidPerMonth		INT,
	@Terms					INT,
	@VacationDays			INT,
	@BonusAmount			DECIMAL(18,2),
	@BonusHoursToCollect	INT,
	@DefaultHoursPerDay		DECIMAL(18,2),
	@StartDate				DATETIME
AS
BEGIN
	
	DECLARE @Today DATETIME

	SELECT @Today = dbo.GettingPMTime(GETDATE())

	SELECT @Today = CONVERT(TIME,@Today)

	SET @StartDate = ISNULL(CONVERT(DATE, @StartDate), '1900-01-01')
	
	IF @PersonId IS NULL
	BEGIN
	
		IF EXISTS (SELECT 1 FROM Person WHERE FirstName = @FirstName AND LastName = @LastName)
		BEGIN
		
			DECLARE @ErrorMessage NVARCHAR(500)
			-- Person First and Last Name uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
			RAISERROR (@ErrorMessage, 16, 1)
		END
		ELSE
		BEGIN

			EXEC dbo.SessionLogPrepare @userLogin = @UserLogin

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
		
			INSERT INTO Person(FirstName, LastName, EmployeeNumber, PTODaysPerAnnum, IsStrawman,HireDate)
			VALUES (@FirstName, @LastName, @EmployeeNumber, 0, 1, @Today) --For strawmem we will use HireDate field as created date field
		
			SET @PersonId = SCOPE_IDENTITY()

			EXEC dbo.SessionLogUnprepare
		END

	END

	IF @PersonId IS NOT NULL
	BEGIN
		IF EXISTS (SELECT 1 FROM Person WHERE PersonId = @PersonId AND ( FirstName <> @FirstName OR LastName <> @LastName ) )
		BEGIN
			UPDATE Person
			SET FirstName = @FirstName, LastName = @LastName
			WHERE PersonId = @PersonId
		END
		
		IF NOT EXISTS (SELECT 1 FROM Pay P WHERE Person = @PersonId)
		BEGIN
			INSERT INTO Pay(Person,StartDate, EndDate, Amount, Timescale, TimesPaidPerMonth, Terms, VacationDays, BonusAmount, BonusHoursToCollect, DefaultHoursPerDay)
			SELECT @PersonId, @StartDate , CONVERT(DATE, dbo.GetFutureDate()), @Amount, @Timescale, @TimesPaidPerMonth, @Terms, @VacationDays, @BonusAmount, @BonusHoursToCollect, @DefaultHoursPerDay
		END
		ELSE IF EXISTS (SELECT 1 FROM Pay pa 
							JOIN  dbo.Person P ON P.PersonId = Pa.Person AND P.IsStrawman = 1
							 WHERE pa.Person = @PersonId AND  pa.StartDate = @StartDate
						 )
		BEGIN
			--IF Saving Second time in a day. then update previous saved on the same day. 
			--OR Updating existing compensation on that startdate.
			UPDATE pa
			Set pa.Amount = @Amount,
				pa.Timescale = @Timescale,
				Pa.TimesPaidPerMonth = @TimesPaidPerMonth,
				Pa.Terms = @Terms,
				pa.VacationDays = @VacationDays,
				pa.BonusAmount = @BonusAmount,
				Pa.BonusHoursToCollect = @BonusHoursToCollect,
				Pa.DefaultHoursPerDay = @DefaultHoursPerDay
			FROM  Pay Pa
			JOIN  dbo.Person P ON P.PersonId = Pa.Person
			WHERE Pa.Person = @PersonId AND P.IsStrawman = 1 AND
			pa.StartDate = @StartDate
		END
		ELSE --To update existing compensation enddate to this startdate and create new compensation from startdate to futuredate.
		BEGIN
			
			DECLARE @FutureDate  DATETIME
			SELECT @FutureDate = CONVERT(DATE, dbo.GetFutureDate())
			IF EXISTS (SELECT 1 FROM dbo.Pay Pa
						WHERE 
						Pa.EndDate = @FutureDate
						AND Pa.Person = @PersonId 
						AND
						(pa.Amount <> @Amount OR
						pa.Timescale <> @Timescale OR
						Pa.TimesPaidPerMonth <> @TimesPaidPerMonth OR
						Pa.Terms <> @Terms OR
						pa.VacationDays <> @VacationDays OR
						pa.BonusAmount <> @BonusAmount OR
						Pa.BonusHoursToCollect <> @BonusHoursToCollect OR
						Pa.DefaultHoursPerDay <> @DefaultHoursPerDay)					 
					 )
			BEGIN

					--End the Previous compensation upto @Startdate.
					UPDATE Pay
					SET EndDate = @StartDate
					WHERE Person = @PersonId AND @StartDate BETWEEN ISNULL(StartDate, '1900-01-01') AND ISNULL(EndDate, dbo.GetFutureDate())

					--Insert new compensation with startdate today.
					INSERT INTO Pay(Person,StartDate, EndDate, Amount, Timescale, TimesPaidPerMonth, Terms, VacationDays, BonusAmount, BonusHoursToCollect, DefaultHoursPerDay)
					SELECT @PersonId, @StartDate, CONVERT(DATE, dbo.GetFutureDate()), @Amount, @Timescale, @TimesPaidPerMonth, @Terms, @VacationDays, @BonusAmount, @BonusHoursToCollect, @DefaultHoursPerDay
			END
		END
				
	END

	SELECT @PersonId
END
