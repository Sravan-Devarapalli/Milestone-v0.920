--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-26-2008
-- Update by:	sainathc
-- Update date:	05/31/2012
-- Description:	Saves a current payment for the specified person.
-- =============================================
CREATE PROCEDURE [dbo].[PaySave]
(
	@PersonId            INT,
	@Amount              DECIMAL(18,2),
	@Timescale           INT,
	@VacationDays        INT,
	@BonusAmount         DECIMAL(18,2),
	@BonusHoursToCollect INT,
	@StartDate           DATETIME,
	@EndDate             DATETIME,
	@OLD_StartDate       DATETIME,
	@OLD_EndDate         DATETIME,
	@PracticeId			 INT,
	@TitleId			 INT,
	@SLTApproval		 BIT,
	@SLTPTOApproval		 BIT,
	@UserLogin			 NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON
	/*
		1.Basic Validations
			a."Person cannot have the compensation for the days before his hire date."
			b."The record overlaps from beginning"
			c."The records overlaps from ending"
			d."The record overlaps within the period"
		2.Check weather to do Update Operation OR Insert Operation (i.e. weather the pay exists Pay Granularity(PERSONID,PAYSTARTDATE))
			a.Update Operation
			   	(i)  Get the Previous Pay record (IFF the updating record STARTDATE is equal to PREVIOUSENDDATE record)
				(ii) If previous record exists update the previous record ENDDATE with the NEWSTARTDATE.
				(iii)Get the Next Pay record (IFF the updating record EndDate is equal to NextStartDATE record)
				(iv) If Next record exists update the Next record StartDate with the NEWEndDATE.
				(v) Update the Pay which has oldStartdate And oldEnddate With all the other parameters.
			b.Insert Operation	
				(i)  Get the Previous Pay record (IFF the updating record STARTDATE is equal to PREVIOUSENDDATE record)
				(ii) If previous record exists update the previous record ENDDATE with the NEWSTARTDATE.
				(iii) Insert New the Pay With all given parameters.
		3.If Today is in between given Pay STARTDATE and ENDDATE 
			a.Update the person default practice and title with the given practiceId and titleId parameters.
			b.Update the IsActivePay in the pay table.
		4.Else If Today is in between given Pay Old_STARTDATE and Old_ENDDATE 
			a.Update the person default practice and title with the practiceId and titleId of the Old_STARTDATE and Old_ENDDATE Pay.
			b.Update the IsActivePay in the pay table.
		5.NO compensation is active we need to consider next future compensation as active and update the Default practiceId and titleId in person table.

	*/

	DECLARE @ErrorMessage NVARCHAR(2048) 
	, @PersonHireDate DATETIME 
	, @W2SalaryId INT
	, @W2HourlyId INT
	, @Today DATETIME
	, @CurrentPMTime DATETIME
	, @UserId INT
	, @TerminationDate DATETIME
	, @PreviousRecordStartDate DATETIME
	, @NextRecordEndDate DateTIME
	, @TempEndDate DATETIME
	, @FutureDate	DATETIME
	, @HoursPerYear	DECIMAL
	, @FirstCompensationStartDate DATETIME
	, @HolidayTimeTypeId INT 
	, @IsPersonRehireDueToPay BIT = CONVERT (BIT,0)
	, @HolidayChargeCodeId INT
 
	
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE()))),
		   @CurrentPMTime = dbo.InsertingTime(),
		   @FutureDate = dbo.GetFutureDate(),
		   @HoursPerYear = dbo.GetHoursPerYear(),
		   @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
	SELECT @HolidayChargeCodeId = CC.Id FROM dbo.ChargeCode CC WHERE CC.Id = @HolidayTimeTypeId
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId  = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'
	SELECT @UserId = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @TerminationDate = TerminationDate FROM Person WHERE PersonId = @PersonId
	SELECT @EndDate = ISNULL(@EndDate, @FutureDate),
		   @OLD_EndDate = ISNULL(@OLD_EndDate, @FutureDate)
	SELECT @PersonHireDate = Hiredate FROM dbo.Person WHERE PersonId = @PersonId

	--1.Basic Validations
	IF (@PersonHireDate > @StartDate)
	BEGIN
		SELECT  @ErrorMessage = 'Person cannot have the compensation for the days before his hire date.'
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END

	IF EXISTS(SELECT 1
	            FROM dbo.Pay
	           WHERE Person = @PersonId 
	             AND StartDate >= @StartDate
	             AND StartDate <> @OLD_StartDate
	             AND ISNULL(EndDate, @FutureDate) <= @OLD_StartDate)
	BEGIN
		-- The record overlaps from beginning
		SELECT  @ErrorMessage = [dbo].[GetErrorMessage](70005)
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.Pay
	                WHERE Person = @PersonId 
	                  AND EndDate <= @EndDate
	                  AND ISNULL(EndDate, @FutureDate) <> @OLD_EndDate
	                  AND StartDate >= @OLD_EndDate)
	BEGIN
		-- The records overlaps from ending
		SELECT  @ErrorMessage = [dbo].[GetErrorMessage](70007)
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.Pay
	                WHERE Person = @PersonId
	                  AND StartDate <= @StartDate AND ISNULL(EndDate, @FutureDate) >= @EndDate
	                  AND StartDate <> @OLD_StartDate AND ISNULL(EndDate, @FutureDate) <> @OLD_EndDate)
	BEGIN
		-- The record overlaps within the period
		SELECT  @ErrorMessage = [dbo].[GetErrorMessage](70008)
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END

	BEGIN TRY
	BEGIN TRAN Tran_PaySave	

	IF EXISTS(SELECT 1
	            FROM dbo.Pay
	           WHERE Person = @PersonId AND StartDate = @OLD_StartDate AND EndDate = @OLD_EndDate)
	BEGIN
	
		-- Auto-adjust a previous record
		DECLARE @PrevRecordEndtDate DATETIME

		SELECT @PrevRecordEndtDate = MAX(EndDate) from  dbo.Pay where Person = @PersonId AND EndDate <=  @OLD_StartDate 

		UPDATE dbo.Pay
			SET EndDate = @StartDate
			WHERE Person = @PersonId AND EndDate = @PrevRecordEndtDate AND  @PrevRecordEndtDate >= @StartDate
		 
		-- Auto-adjust a next record
		DECLARE @NextRecordStartDate DATETIME

		SELECT @NextRecordStartDate = MIN(StartDate) from  dbo.Pay where Person = @PersonId AND StartDate >=  @OLD_EndDate
		
		UPDATE dbo.Pay
		SET StartDate = @EndDate
		WHERE Person = @PersonId AND StartDate = @NextRecordStartDate AND @NextRecordStartDate <= @EndDate

		UPDATE dbo.Pay
		   SET Amount = @Amount,
		       Timescale = @Timescale,
			   VacationDays = @VacationDays,
			   BonusAmount = @BonusAmount,
			   BonusHoursToCollect = ISNULL(@BonusHoursToCollect, @HoursPerYear),
			   TitleId = @TitleId,
			   PracticeId = @PracticeId,
		       StartDate = @StartDate,
		       EndDate = @EndDate,
			   SLTApproval = @SLTApproval,
			   SLTPTOApproval = @SLTPTOApproval
		 WHERE Person = @PersonId AND StartDate = @OLD_StartDate AND EndDate = @OLD_EndDate

	END
	ELSE
	BEGIN
		-- Auto-adjust a previous record
		UPDATE dbo.Pay
		   SET EndDate = @StartDate
		 WHERE Person = @PersonId AND EndDate > @StartDate
	
		INSERT INTO dbo.Pay
					(Person, StartDate, EndDate, Amount, Timescale,
					 VacationDays, BonusAmount, BonusHoursToCollect,PracticeId,TitleId,SLTApproval,SLTPTOApproval)
			 VALUES (@PersonId, @StartDate, @EndDate, @Amount, @Timescale, 
					 @VacationDays, @BonusAmount, ISNULL(@BonusHoursToCollect, @HoursPerYear),@PracticeId,@TitleId,@SLTApproval,@SLTPTOApproval)

	END


	--DECLARE @Today DATETIME
	--SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))
	IF (@Today >= @StartDate AND @Today < @EndDate)
	BEGIN
		
		UPDATE Person
		SET TitleId = @TitleId,
		DefaultPractice = @PracticeId
		WHERE PersonId = @PersonId

		UPDATE Pay
		SET IsActivePay = CASE WHEN StartDate = @StartDate AND  EndDate = @EndDate 
							   THEN 1 ELSE 0 END
		WHERE Person = @PersonId

	END
	ELSE IF(@Today >= @OLD_StartDate AND @Today < @OLD_EndDate)
	BEGIN
		UPDATE P
			SET P.TitleId = Pa.TitleId,
			P.DefaultPractice = Pa.PracticeId
			FROM dbo.Person P
			JOIN dbo.Pay Pa
			ON P.PersonId = Pa.Person AND 
			pa.StartDate <= @Today AND ISNULL(EndDate, @FutureDate) > @Today
			WHERE P.PersonId = @PersonId

			UPDATE dbo.Pay
			SET IsActivePay = CASE WHEN StartDate <= @Today AND  EndDate > @Today
								   THEN 1 ELSE 0 END
			WHERE Person = @PersonId
	END
	--5.NO compensation is active we need to consider next future compensation as active and update the Default practiceId and titleId in person table.
	IF((SELECT COUNT(*) FROM dbo.Pay where @Today BETWEEN StartDate AND EndDate-1 and Person = @PersonId) = 0)
	BEGIN
		UPDATE P
			SET P.TitleId = Pa.TitleId,
			P.DefaultPractice = Pa.PracticeId
			FROM dbo.Person P
			JOIN dbo.Pay Pa
			ON P.PersonId = Pa.Person AND 
			pa.StartDate = (SELECT MIN(StartDate) FROM dbo.Pay where Person = @PersonId and StartDate > @Today)
			WHERE P.PersonId = @PersonId
	END

	SELECT @PreviousRecordStartDate = StartDate
	FROM dbo.Pay
	WHERE EndDate = @StartDate
			AND Person = @PersonId
	SELECT @NextRecordEndDate = EndDate
	FROM dbo.Pay
	WHERE StartDate = @EndDate
			AND Person = @PersonId

	DELETE PC
	FROM dbo.PersonCalendar PC
	JOIN dbo.Calendar C ON (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,CASE WHEN @StartDate > @OLD_StartDate  THEN @OLD_StartDate ELSE @StartDate END) AND 
							ISNULL(@NextRecordEndDate,CASE WHEN @EndDate < @OLD_EndDate THEN @OLD_EndDate ELSE @EndDate END)-1)
		AND PC.PersonId = @PersonId AND PC.Date = C.Date
	JOIN dbo.TimeType TT ON TT.TimeTypeId = PC.TimeTypeId
	LEFT JOIN Pay P ON P.Person = PC.PersonId AND C.Date BETWEEN p.StartDate AND P.EndDate - 1
	WHERE (
			P.Person IS NULL --If pay is deleted we need to delete the administrative time entries
			OR  
			--If pay is Updated we need to delete the administrative time entries according to the salary type and hourly type
			NOT 
			(
				(p.Timescale = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
				OR 
				(p.Timescale = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
			)
		 )

	--Delete Holiday timeEntries if person is not w2salaried.
	DELETE TEH
	FROM dbo.TimeEntryHours TEH
	JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId
	JOIN dbo.Calendar C ON (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,CASE WHEN @StartDate > @OLD_StartDate  THEN @OLD_StartDate ELSE @StartDate END) AND 
							ISNULL(@NextRecordEndDate,CASE WHEN @EndDate < @OLD_EndDate THEN @OLD_EndDate ELSE @EndDate END)-1)
		AND TE.PersonId = @PersonId AND TE.ChargeCodeDate = C.Date
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
	LEFT JOIN Pay P ON P.Person = TE.PersonId AND C.Date BETWEEN p.StartDate AND P.EndDate - 1
	WHERE (
			P.Person IS NULL --If pay is deleted we need to delete the administrative time entries
			OR  
			--If pay is Updated we need to delete the administrative time entries according to the salary type and hourly type
			NOT 
			(
				(p.Timescale = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
				OR 
				(p.Timescale = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
			)
		 )

	DELETE TE
	FROM dbo.TimeEntry TE 
	JOIN dbo.Calendar C ON (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,CASE WHEN @StartDate > @OLD_StartDate  THEN @OLD_StartDate ELSE @StartDate END) AND 
							ISNULL(@NextRecordEndDate,CASE WHEN @EndDate < @OLD_EndDate THEN @OLD_EndDate ELSE @EndDate END)-1)
		AND TE.PersonId = @PersonId AND TE.ChargeCodeDate = C.Date
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
	LEFT JOIN Pay P ON P.Person = TE.PersonId AND C.Date BETWEEN p.StartDate AND P.EndDate - 1
	WHERE (
			P.Person IS NULL --If pay is deleted we need to delete the administrative time entries
			OR  
			--If pay is Updated we need to delete the administrative time entries according to the salary type and hourly type
			NOT 
			(
				(p.Timescale = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
				OR 
				(p.Timescale = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
			)
		 )
	
	--Insert PTO/Holiday timeEntries
	INSERT INTO dbo.TimeEntry(PersonId,
							ChargeCodeId,
							ChargeCodeDate,
							ForecastedHours,
							Note,
							IsCorrect,
							IsAutoGenerated)
	SELECT DISTINCT @PersonId
		,CC.Id
		,C.Date
		,0
		, ISNULL(C.HolidayDescription, ISNULL(PC.Description, ''))
		,1
		,1 --Here it is Auto generated.
	FROM dbo.Calendar C
	JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate AND P.EndDate-1 AND p.Timescale IN (@W2SalaryId,@W2HourlyId) AND p.Person = @PersonId
	JOIN dbo.Person per ON per.PersonId = p.Person AND per.IsStrawman = 0
	LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = p.Person AND PC.DayOff = 1
	INNER JOIN dbo.TimeType TT ON  (C.Date <= @TerminationDate OR @TerminationDate IS NULL)
								AND (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,@StartDate) AND ISNULL(@NextRecordEndDate,@EndDate)-1)
								AND ((C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId )
									OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND TT.TimeTypeId = PC.TimeTypeId)
									)
	INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
	LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = p.Person AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = C.Date
	WHERE	(
				TE.TimeEntryId IS NULL
				AND  
				--If pay is Updated we need to delete the administrative time entries according to the salary type and hourly type
				(
					(p.Timescale = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
					OR 
					(p.Timescale = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
				)
			)
		
	INSERT INTO dbo.TimeEntryHours(TimeEntryId,
									ActualHours,
									IsChargeable,
									CreateDate,
									ModifiedDate,
									ModifiedBy,
									ReviewStatusId)
	SELECT TE.TimeEntryId
			,CASE WHEN TT.TimeTypeId = @HolidayTimeTypeId THEN 8
					ELSE ISNULL(PC.ActualHours,8) END
			,0 --Non Billable
			,@CurrentPMTime
			,@CurrentPMTime
			,@UserId
			,CASE WHEN C.DayOff = 1 OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND PC.TimeTypeId <> @HolidayTimeTypeId AND PC.IsFromTimeEntry <> 1) THEN 2 
					ELSE 1 END -- Holiday:Approved, PTO: from CalendarPage Approved, Floating-Holiday: pending
	FROM dbo.Calendar C
	JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate AND P.EndDate-1 AND p.Timescale IN (@W2SalaryId,@W2HourlyId) AND p.Person = @PersonId
	JOIN dbo.Person per ON per.PersonId = p.Person AND per.IsStrawman = 0
	LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = p.Person AND PC.DayOff = 1
	INNER JOIN dbo.TimeType TT ON (C.Date <= @TerminationDate OR @TerminationDate IS NULL)
								AND (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,@StartDate) AND ISNULL(@NextRecordEndDate,@EndDate)-1)
								AND ((C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId )
									OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND TT.TimeTypeId = PC.TimeTypeId)
									)
	INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
	INNER JOIN dbo.TimeEntry TE ON TE.PersonId = p.Person AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = C.Date
	LEFT JOIN dbo.TimeEntryHours TEH ON  TEH.TimeEntryId = TE.TimeEntryId						   
	WHERE	(
			TEH.TimeEntryId IS NULL
			AND  
			--If pay is Updated we need to delete the administrative time entries according to the salary type and hourly type
			(
				(p.Timescale = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
				OR 
				(p.Timescale = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
			)
			)


	DECLARE @NeedToDeleteDates TABLE (Date DATETIME,SubstituteDate DATETIME)

	INSERT INTO @NeedToDeleteDates (Date,SubstituteDate)
	SELECT PC.Date,PC.SubstituteDate
	FROM PersonCalendar PC
	INNER JOIN Pay P ON P.Person = PC.PersonId 
						AND P.Person = @PersonId
						AND (PC.Date BETWEEN p.StartDate AND P.EndDate - 1 OR PC.SubstituteDate BETWEEN p.StartDate AND P.EndDate - 1)
						AND PC.SubstituteDate IS NOT NULL
						AND PC.TimeTypeId IS NULL
						AND p.Timescale != @W2SalaryId --W2-Salary 
						AND PC.DayOff = 0

	DELETE PC
	--SELECT  *
	FROM dbo.PersonCalendar PC
	INNER JOIN @NeedToDeleteDates P ON  PC.PersonId = @PersonId AND ( PC.Date = P.Date OR PC.Date = P.SubstituteDate)

	DELETE TEH
	--SELECT  *
	FROM dbo.TimeEntryHours TEH
	JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId AND TE.PersonId = @PersonId
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.TimeTypeId = @HolidayTimeTypeId
	JOIN @NeedToDeleteDates ND ON  TE.PersonId = @PersonId AND TE.ChargeCodeDate = ND.SubstituteDate

	DELETE TE
	--SELECT  *
	FROM dbo.TimeEntry TE 
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.TimeTypeId = @HolidayTimeTypeId AND  TE.PersonId = @PersonId
	JOIN @NeedToDeleteDates ND ON  TE.PersonId = @PersonId AND TE.ChargeCodeDate = ND.SubstituteDate

	/*
	Rehire Logic
	1.Need to check weather  the first compensation is contract or not
	2.If Yes Check weather the person has any compensation start date as salary type after the contract type before today(i.e. not the future compensation)
	3.If Yes Terminate the person with latest compensation start date with contract type. And
	4.Re-hire the person with hire date as salary type compensation start date.
	*/

	DECLARE @TerminationReasonId INT,@HireDate DATETIME,@PreviousTerminationDate DATETIME,@FirstSalaryCompensationStartDate DATETIME
	SELECT @TerminationReasonId = TR.TerminationReasonId FROM dbo.TerminationReasons TR WHERE TR.TerminationReason = 'Voluntary - 1099 Contract Ended'
	SELECT @PreviousTerminationDate = TerminationDate FROM dbo.Person AS P WHERE P.PersonId = @PersonId

	SELECT @FirstCompensationStartDate = MIN(pay.StartDate)
	FROM dbo.Pay pay WITH(NOLOCK)
	INNER JOIN dbo.Person P ON pay.person = P.personid AND pay.Person = @PersonId
	WHERE pay.StartDate >= P.HireDate

	--Constraint Violation "Salary Type to Contract Type Violation" 
	SELECT @FirstSalaryCompensationStartDate = MIN(pay.StartDate)
	FROM dbo.Pay pay WITH(NOLOCK)
	INNER JOIN dbo.Person P ON pay.person = P.personid AND pay.Person = @PersonId 
	INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale AND T.IsContractType = 0
	WHERE pay.StartDate >= P.HireDate

	--Check weather the person has any compensation start date as contract type after the First salary type
	IF EXISTS (SELECT 1
				FROM dbo.Pay pay 
				INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale
				WHERE pay.Person = @PersonId AND pay.StartDate > @FirstSalaryCompensationStartDate AND T.IsContractType = 1
				)
	BEGIN
		SELECT  @ErrorMessage = 'Salary Type to Contract Type Violation'
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END
		
	--1.Need to check weather  the first compensation is contract or not
	IF EXISTS (SELECT 1
				FROM dbo.Pay pay WITH(NOLOCK)
				INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale
				WHERE pay.Person = @PersonId AND @FirstCompensationStartDate = pay.StartDate AND T.IsContractType = 1
				)
	BEGIN
	--2.If Yes Check weather the person has any compensation start date as salary type after the contract type before today(i.e. not the future compensation)
		IF EXISTS (SELECT 1
					FROM dbo.Pay pay WITH(NOLOCK)
					INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale
					WHERE pay.Person = @PersonId AND pay.StartDate > @FirstCompensationStartDate AND T.IsContractType = 0 AND pay.StartDate <= @Today
					)
		BEGIN
				
	--3.If Yes Terminate the person with latest compensation end date  with contract type.
			SELECT @TerminationDate = MAX(pay.EndDate) -1 
			FROM dbo.Pay pay WITH(NOLOCK)
			INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale
			WHERE pay.Person = @PersonId AND pay.StartDate >= @FirstCompensationStartDate AND T.IsContractType = 1 AND pay.StartDate < @Today
				
			EXEC [dbo].[PersonTermination] @PersonId = @PersonId , @TerminationDate = @TerminationDate , @PersonStatusId = 2 , @FromPaySaveSproc = 1 -- terminating the person

				
			-- Ensure the temporary table exists
			EXEC SessionLogPrepare @UserLogin = @UserLogin

			UPDATE dbo.Person
				SET TerminationDate = @TerminationDate,
					PersonStatusId = 2,
					TitleId = @TitleId,
					PracticeOwnedId = @PracticeId,
					TerminationReasonId = @TerminationReasonId
				WHERE PersonId = @PersonId

			EXEC dbo.PersonStatusHistoryUpdate
				@PersonId = @PersonId,
				@PersonStatusId = 2

			EXEC [dbo].[AdjustTimeEntriesForTerminationDateChanged] @PersonId = @PersonId, @TerminationDate = @TerminationDate, @PreviousTerminationDate = @PreviousTerminationDate,@UserLogin = @UserLogin	

	--4.Re-hire the person with hire date as salary type compensation start date.
			SELECT @HireDate = MIN(pay.StartDate),
			       @IsPersonRehireDueToPay = 1
			FROM dbo.Pay pay WITH(NOLOCK)
			INNER JOIN dbo.Timescale T ON T.TimescaleId = pay.Timescale
			WHERE pay.Person = @PersonId AND @FirstCompensationStartDate <= pay.StartDate AND T.IsContractType = 0 AND pay.StartDate < @Today

				
			-- Ensure the temporary table exists
			EXEC SessionLogPrepare @UserLogin = @UserLogin

			UPDATE dbo.Person
				SET HireDate = ISNULL(@HireDate,@StartDate),
					TerminationDate = NULL,
					PersonStatusId = 1,
					TerminationReasonId = NULL
				WHERE PersonId = @PersonId

			EXEC dbo.PersonStatusHistoryUpdate
				@PersonId = @PersonId,
				@PersonStatusId = 1

			EXEC [dbo].[AdjustTimeEntriesForTerminationDateChanged] @PersonId = @PersonId, @TerminationDate = @TerminationDate, @PreviousTerminationDate = @TerminationDate,@UserLogin = @UserLogin	

		END			
	END

	COMMIT TRAN Tran_PaySave

	SELECT @IsPersonRehireDueToPay AS IsPersonRehireDueToPay

	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_PaySave
		DECLARE	 @ERROR_STATE			tinyint
		,@ERROR_SEVERITY		tinyint
		,@ERROR_MESSAGE		    nvarchar(2000)
		,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)

	END CATCH

GO

