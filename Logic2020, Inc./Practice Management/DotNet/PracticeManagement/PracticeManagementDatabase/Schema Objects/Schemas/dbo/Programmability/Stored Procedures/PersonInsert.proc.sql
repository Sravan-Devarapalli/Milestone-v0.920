﻿-- =============================================
-- Author:		
-- Create date: 
-- Updated by : Srinivas.M
-- Update Date: 05-21-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonInsert]
(
	@FirstName       NVARCHAR(40),
	@LastName        NVARCHAR(40), 
	@PTODaysPerAnnum INT,
	@HireDate        DATETIME,
	@TerminationDate DATETIME,
	@Alias           NVARCHAR(100),
	@DefaultPractice INT,
	@PersonStatusId	 INT,
	@SeniorityId     INT,
	@UserLogin       NVARCHAR(255),
	@ManagerId		 INT = NULL,
	@PracticeOwnedId INT = NULL,
	@PersonId        INT OUTPUT,
	@TelephoneNumber NVARCHAR(20) = NULL,
	@PaychexID		 NVARCHAR(20),
	@IsOffshore	     BIT,
	@PersonDivisionId	INT,
	@TerminationReasonId	INT
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048),
			@Today			DATETIME

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	IF EXISTS(SELECT 1
	            FROM dbo.[Person] AS p
	           WHERE p.[LastName] = @LastName AND p.[FirstName] = @FirstName)
	BEGIN
		-- Person First and Last Name uniqueness violation
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.[Person] AS p
	                WHERE p.[Alias] = @Alias)
	BEGIN
		-- Person Email uniqueness violation
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70002)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		-- Generating Employee Number
		DECLARE @EmployeeNumber NVARCHAR(12)
		DECLARE @StringCounter NVARCHAR(7)
		DECLARE @Counter INT
		DECLARE @PracticeManagerId INT

		SET @Counter = 0

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

		-- From #1663: if you save the person record, and line manager is blank, but practice is set, set the Line Manager to the person's Practice.PracticeManager.
		IF @DefaultPractice IS NOT NULL AND @ManagerId IS NULL
			-- Try to get default manager id
			SELECT @ManagerId = p.PracticeManagerId FROM practice as p WHERE p.PracticeId = @DefaultPractice
			
		-- From #1663: if you save the person record, and both line manager and practice are blank, set the Line Manager to the default line manager value.
		IF @ManagerId IS NULL AND @DefaultPractice IS NULL
			-- Try to get default manager id
			SELECT @ManagerId = p.PersonId FROM person AS p WHERE p.IsDefaultManager = 1

		SELECT @PersonStatusId = CASE WHEN @TerminationDate < @Today THEN 2 ELSE @PersonStatusId END
		-- Inserting Person
		INSERT dbo.Person
			(FirstName, LastName, PTODaysPerAnnum,  HireDate,  Alias, DefaultPractice, 
		     PersonStatusId, EmployeeNumber, TerminationDate, SeniorityId, ManagerId, PracticeOwnedId, TelephoneNumber,IsStrawman,IsOffshore,PaychexID, DivisionId, TerminationReasonId)
		VALUES
			(@FirstName, @LastName, @PTODaysPerAnnum, @HireDate, @Alias, @DefaultPractice, 
		     @PersonStatusId, @EmployeeNumber, @TerminationDate, @SeniorityId, @ManagerId, @PracticeOwnedId, @TelephoneNumber,0,@IsOffshore,@PaychexID, @PersonDivisionId, @TerminationReasonId)

		-- End logging session
		EXEC dbo.SessionLogUnprepare

		SET @PersonId = SCOPE_IDENTITY()

		DECLARE @Date DATETIME 

		SET @Date = @Today

		IF(@HireDate < @Today)
		BEGIN

		SET @Date = CONVERT(DATE,@HireDate)

		END

		INSERT INTO [dbo].[PersonStatusHistory]
			   ([PersonId]
			   ,[PersonStatusId]
			   ,[StartDate]
			   )
			VALUES
			   (@PersonId
			   ,@PersonStatusId
			   ,@Date
			   )

		SELECT @PersonId
	END

