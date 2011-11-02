CREATE PROCEDURE [dbo].[SaveStrawManFromExisting]
	@FirstName				NVARCHAR(100),
	@LastName				NVARCHAR(100),
	@PersonId				INT OUTPUT
AS
BEGIN
	
	DECLARE @Today DATETIME,
	        @ExistingPersonId INT
	Select  @ExistingPersonId = @PersonId
	SELECT @Today = dbo.GettingPMTime(GETDATE())

	SELECT @Today = CONVERT(TIME,@Today)

	declare @StartDate DATETIME
	SELECT @StartDate = '1900-01-01'
	
		IF EXISTS (SELECT 1 FROM Person WHERE FirstName = @FirstName AND LastName = @LastName)
		BEGIN
		
			DECLARE @ErrorMessage NVARCHAR(500)
			-- Person First and Last Name uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
			RAISERROR (@ErrorMessage, 16, 1)
		END
		ELSE
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
		
			INSERT INTO Person(FirstName, LastName, EmployeeNumber, PTODaysPerAnnum, IsStrawman,HireDate)
			VALUES (@FirstName, @LastName, @EmployeeNumber, 0, 1, @Today) --For strawmem we will use HireDate field as created date field
		
			SET @PersonId = SCOPE_IDENTITY()
		END
	
		
		IF NOT EXISTS (SELECT 1 FROM Pay P WHERE Person = @PersonId)
		BEGIN
			INSERT INTO Pay(Person,StartDate,EndDate,Amount,Timescale,TimesPaidPerMonth,Terms,VacationDays,BonusAmount,BonusHoursToCollect,DefaultHoursPerDay,SeniorityId,PracticeId,IsActivePay)
			SELECT @PersonId,StartDate,EndDate,Amount,Timescale,TimesPaidPerMonth,Terms,VacationDays,BonusAmount,BonusHoursToCollect,DefaultHoursPerDay,SeniorityId,PracticeId,IsActivePay FROM dbo.Pay
     		 where Person = @ExistingPersonId
		END
		SELECT @PersonId
END
