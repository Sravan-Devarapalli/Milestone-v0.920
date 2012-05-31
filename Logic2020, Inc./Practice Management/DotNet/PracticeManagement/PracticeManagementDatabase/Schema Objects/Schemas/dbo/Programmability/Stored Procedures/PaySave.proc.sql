﻿--The source is 
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
	@TimesPaidPerMonth   INT,
	@Terms               INT,
	@VacationDays        INT,
	@BonusAmount         DECIMAL(18,2),
	@BonusHoursToCollect INT,
	@DefaultHoursPerDay  DECIMAL(18,2),
	@StartDate           DATETIME,
	@EndDate             DATETIME,
	@OLD_StartDate       DATETIME,
	@OLD_EndDate         DATETIME,
	@SeniorityId		 INT,
	@PracticeId			 INT,
	@SalesCommissionFractionOfMargin DECIMAL(18,2),
	@UserLogin			 NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @ErrorMessage NVARCHAR(2048) 
	, @RowsEffected INT
	, @PersonHireDate DATETIME 
	, @W2SalaryId INT
	, @Today DATETIME
	, @CurrentPMTime DATETIME
	, @UserId INT
	, @TerminationDate DATETIME
	, @PreviousRecordStartDate DATETIME
	, @NextRecordEndDate DateTIME
	, @TempEndDate DATETIME

 
	
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
	SET @CurrentPMTime = dbo.InsertingTime()
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @UserId = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @TerminationDate = TerminationDate FROM Person WHERE PersonId = @PersonId
	
	SELECT @PersonHireDate = Hiredate
	FROM dbo.Person
	WHERE PersonId = @PersonId
	
	IF (@PersonHireDate > @StartDate)
	BEGIN
		SELECT  @ErrorMessage = 'Person cannot have the compensation for the days before his hire date.'
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END

	SELECT @EndDate = ISNULL(@EndDate, dbo.GetFutureDate())
	SELECT @OLD_EndDate = ISNULL(@OLD_EndDate, dbo.GetFutureDate())

	IF EXISTS(SELECT 1
	            FROM dbo.Pay
	           WHERE Person = @PersonId 
	             AND StartDate >= @StartDate
	             AND StartDate <> @OLD_StartDate
	             AND ISNULL(EndDate, dbo.GetFutureDate()) <= @OLD_StartDate)
	BEGIN
		-- The record overlaps from begining
		SELECT  @ErrorMessage = [dbo].[GetErrorMessage](70005)
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.Pay
	                WHERE Person = @PersonId 
	                  AND EndDate <= @EndDate
	                  AND ISNULL(EndDate, dbo.GetFutureDate()) <> @OLD_EndDate
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
	                  AND StartDate <= @StartDate AND ISNULL(EndDate, dbo.GetFutureDate()) >= @EndDate
	                  AND StartDate <> @OLD_StartDate AND ISNULL(EndDate, dbo.GetFutureDate()) <> @OLD_EndDate)
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
	

		DELETE DF
		FROM  dbo.[DefaultCommission] DF
		LEFT JOIN dbo.Pay P ON DF.StartDate = P.StartDate AND DF.PersonId = P.Person  
		WHERE DF.StartDate >= @StartDate AND  Type =1 AND DF.PersonId = @PersonId AND P.Person IS NULL
				AND DF.StartDate < @EndDate
		
		-- Auto-adjust a previous record
		DECLARE @PrevRecordEndtDate DATETIME

		SELECT @PrevRecordEndtDate = MAX(EndDate) from  dbo.Pay where Person = @PersonId AND EndDate <=  @OLD_StartDate 

			UPDATE dbo.Pay
			SET EndDate = @StartDate
			WHERE Person = @PersonId AND EndDate = @PrevRecordEndtDate AND  @PrevRecordEndtDate >= @StartDate
		 
		UPDATE dbo.[DefaultCommission]
		SET EndDate = @StartDate
		WHERE PersonId = @PersonId
			   AND [Type] = 1 -- Sales Commission
			   AND EndDate = @OLD_StartDate

		-- Auto-adjust a next record
		DECLARE @NextRecordStartDate DATETIME

		SELECT @NextRecordStartDate = MIN(StartDate) from  dbo.Pay where Person = @PersonId AND StartDate >=  @OLD_EndDate
		
		UPDATE dbo.Pay
		SET StartDate = @EndDate
		WHERE Person = @PersonId AND StartDate = @NextRecordStartDate AND @NextRecordStartDate <= @EndDate


		UPDATE dbo.[DefaultCommission]
		SET StartDate = @EndDate
		WHERE PersonId = @PersonId
			   AND [Type] = 1 -- Sales Commission
			   AND StartDate = @OLD_EndDate
		
		UPDATE dbo.Pay
		   SET Amount = @Amount,
		       Timescale = @Timescale,
		       TimesPaidPerMonth = @TimesPaidPerMonth,
		       Terms = @Terms,
			   VacationDays = @VacationDays,
			   BonusAmount = @BonusAmount,
			   BonusHoursToCollect = ISNULL(@BonusHoursToCollect, dbo.GetHoursPerYear()),
		       DefaultHoursPerDay = @DefaultHoursPerDay,
			   SeniorityId = @SeniorityId,
			   PracticeId = @PracticeId,
		       StartDate = @StartDate,
		       EndDate = @EndDate
		 WHERE Person = @PersonId AND StartDate = @OLD_StartDate AND EndDate = @OLD_EndDate


		 IF EXISTS (SELECT 1 FROM dbo.[DefaultCommission] 
					WHERE PersonId = @PersonId AND [Type] = 1 
					AND StartDate = @OLD_StartDate
					)
		BEGIN
			IF(@SalesCommissionFractionOfMargin IS NULL)
				DELETE FROM dbo.[DefaultCommission]
				WHERE PersonId = @PersonId AND [Type] = 1 
							AND StartDate = @OLD_StartDate
			ELSE
			BEGIN
				 
				  IF NOT EXISTS (SELECT 1 FROM dbo.[DefaultCommission]
								 WHERE [TYPE]=1 AND PersonId = @PersonId 
								 AND StartDate >  @StartDate)
					AND NOT EXISTS (SELECT 1 FROM dbo.Pay
									WHERE Person= @PersonId AND StartDate > @StartDate
					)
					 SELECT @TempEndDate = '2029-12-31'
				  ELSE
					SELECT @TempEndDate = @EndDate
				 UPDATE  dbo.[DefaultCommission]
				 SET FractionOfMargin = @SalesCommissionFractionOfMargin,
					 StartDate = @StartDate,
					 EndDate = @TempEndDate
				 WHERE PersonId = @PersonId AND [Type] = 1 
								AND StartDate = @OLD_StartDate
			END
		END
		ELSE IF @SalesCommissionFractionOfMargin IS NOT NULL
		BEGIN
			  IF NOT EXISTS (SELECT 1 FROM dbo.[DefaultCommission]
							 WHERE [TYPE]=1 AND PersonId = @PersonId 
							 AND StartDate >  @StartDate)
				AND NOT EXISTS (SELECT 1 FROM dbo.Pay
								WHERE Person= @PersonId AND StartDate > @StartDate
				)
			  SELECT @TempEndDate = '2029-12-31'
			  ELSE
				SELECT @TempEndDate = @EndDate

			INSERT INTO dbo.[DefaultCommission]
							(PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
			VALUES (@PersonId, @StartDate, @TempEndDate, @SalesCommissionFractionOfMargin, 1, NULL)
		END

	END
	ELSE
	BEGIN
		-- Auto-adjust a previous record
		UPDATE dbo.Pay
		   SET EndDate = @StartDate
		 WHERE Person = @PersonId AND EndDate > @StartDate

		DELETE DF
		FROM  dbo.[DefaultCommission] DF
		LEFT JOIN dbo.Pay P ON DF.StartDate = P.StartDate AND DF.PersonId = P.Person  
		WHERE DF.StartDate >= @StartDate AND  Type =1 AND DF.PersonId = @PersonId AND P.Person IS NULL
				AND DF.StartDate < @EndDate
	
		INSERT INTO dbo.Pay
					(Person, StartDate, EndDate, Amount, Timescale, TimesPaidPerMonth, Terms,
					 VacationDays, BonusAmount, BonusHoursToCollect,
					 DefaultHoursPerDay,SeniorityId,PracticeId)
			 VALUES (@PersonId, @StartDate, @EndDate, @Amount, @Timescale, @TimesPaidPerMonth, @Terms,
					 @VacationDays, @BonusAmount, ISNULL(@BonusHoursToCollect, dbo.GetHoursPerYear()),
					 @DefaultHoursPerDay,@SeniorityId,@PracticeId)
		
		

		UPDATE dbo.[DefaultCommission]
			SET EndDate = @StartDate
			WHERE PersonId = @PersonId
				   AND [Type] = 1 -- Sales Commission
				   AND EndDate > @StartDate
				   AND StartDate < @StartDate

		DECLARE @DefaultCommissionEndDate DATETIME
		 

		SELECT @DefaultCommissionEndDate = '2029-12-31'


		IF EXISTS (SELECT 1 FROM dbo.[DefaultCommission] 
					WHERE PersonId = @PersonId AND TYPE = 1 
						AND StartDate > @StartDate)
		SELECT @DefaultCommissionEndDate = @EndDate
		
		IF @SalesCommissionFractionOfMargin IS NOT NULL
		INSERT INTO dbo.[DefaultCommission]
						(PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
			VALUES (@PersonId, @StartDate, @DefaultCommissionEndDate, @SalesCommissionFractionOfMargin, 1, NULL)

	END
	--DECLARE @Today DATETIME
	--SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))
	IF (@Today >= @StartDate AND @Today < @EndDate)
	BEGIN
		
		UPDATE Person
		SET SeniorityId = @SeniorityId,
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
			SET P.SeniorityId = Pa.SeniorityId,
			P.DefaultPractice = Pa.PracticeId
			FROM dbo.Person P
			JOIN dbo.Pay Pa
			ON P.PersonId = Pa.Person AND 
			pa.StartDate <= @Today AND ISNULL(EndDate,dbo.GetFutureDate()) > @Today
			WHERE P.PersonId = @PersonId

			UPDATE dbo.Pay
			SET IsActivePay = CASE WHEN StartDate <= @Today AND  EndDate > @Today
								   THEN 1 ELSE 0 END
			WHERE Person = @PersonId
	END
	--NO compensation is active we need to consider next future compersation as active and update the default practiceid and serniorityid in person table.
	IF((SELECT COUNT(*) FROM dbo.Pay where @Today BETWEEN StartDate AND EndDate-1 and Person = @PersonId) = 0)
	BEGIN
		UPDATE P
			SET P.SeniorityId = Pa.SeniorityId,
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

	DECLARE @HolidayTimeTypeId INT

	SET @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
	
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
	WHERE (P.Person IS NULL OR  p.Timescale <> @W2SalaryId)

	DELETE TE
	FROM dbo.TimeEntry TE 
	JOIN dbo.Calendar C ON (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,CASE WHEN @StartDate > @OLD_StartDate  THEN @OLD_StartDate ELSE @StartDate END) AND 
							ISNULL(@NextRecordEndDate,CASE WHEN @EndDate < @OLD_EndDate THEN @OLD_EndDate ELSE @EndDate END)-1)
		AND TE.PersonId = @PersonId AND TE.ChargeCodeDate = C.Date
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
	LEFT JOIN Pay P ON P.Person = TE.PersonId AND C.Date BETWEEN p.StartDate AND P.EndDate - 1
	WHERE (P.Person IS NULL OR  p.Timescale <> @W2SalaryId)
	
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
			--,CASE WHEN PC.PersonId IS NOT NULL AND PC.DayOff <> C.DayOff AND ISNULL(PC.IsFloatingHoliday,0) = 0 THEN 'PTO'
			--	WHEN PC.PersonId IS NOT NULL AND PC.DayOff <> C.DayOff AND PC.IsFloatingHoliday = 1 THEN 'Floating Holiday'
			--	ELSE ISNULL(C.HolidayDescription,'') END
			,1
			,1 --Here it is Auto generated.
		FROM dbo.Calendar C
		JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate AND P.EndDate-1 AND p.Timescale = @W2SalaryId AND p.Person = @PersonId
		JOIN dbo.Person per ON per.PersonId = p.Person AND per.IsStrawman = 0
		LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = p.Person AND PC.DayOff = 1
		INNER JOIN dbo.TimeType TT ON  (C.Date <= @TerminationDate OR @TerminationDate IS NULL)
									AND (C.Date BETWEEN ISNULL(@PreviousRecordStartDate,@StartDate) AND ISNULL(@NextRecordEndDate,@EndDate)-1)
									AND ((C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId )
										OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND TT.TimeTypeId = PC.TimeTypeId)
										)
		INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
		LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = p.Person AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = C.Date
		WHERE TE.TimeEntryId IS NULL
		
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
		JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate AND P.EndDate-1 AND p.Timescale = @W2SalaryId AND p.Person = @PersonId
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
		WHERE TEH.TimeEntryId IS NULL

	COMMIT TRAN Tran_PaySave
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

