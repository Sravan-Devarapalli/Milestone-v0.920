--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-26-2008
-- Update by:	Anatoliy Lokshin
-- Update date:	7-3-2008
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
	@SalesCommissionFractionOfMargin DECIMAL(18,2)
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @ErrorMessage NVARCHAR(2048) 
	,@RowsEffected INT
	DECLARE @PersonHireDate DATETIME 
	
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

	BEGIN TRAN

	IF EXISTS(SELECT 1
	            FROM dbo.Pay
	           WHERE Person = @PersonId AND StartDate = @OLD_StartDate AND EndDate = @OLD_EndDate)
	BEGIN
		-- Auto-adjust a previous record
		UPDATE dbo.Pay
		   SET EndDate = @StartDate
		 WHERE Person = @PersonId AND EndDate = @OLD_StartDate

		UPDATE dbo.[DefaultCommission]
		SET EndDate = @StartDate
		WHERE PersonId = @PersonId
			   AND [Type] = 1 -- Sales Commission
			   AND EndDate = @OLD_StartDate

		-- Auto-adjust a next record
		UPDATE dbo.Pay
		SET StartDate = @EndDate
		WHERE Person = @PersonId AND StartDate = @OLD_EndDate

		--SELECT @RowsEffected = @@ROWCOUNT

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
			 UPDATE  dbo.[DefaultCommission]
			 SET FractionOfMargin = @SalesCommissionFractionOfMargin,
				 StartDate = @StartDate,
				 EndDate = @EndDate
			 WHERE PersonId = @PersonId AND [Type] = 1 
							AND StartDate = @OLD_StartDate
		END
		ELSE IF @SalesCommissionFractionOfMargin IS NOT NULL
		BEGIN
			  DECLARE @TempEndDate DATETIME
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

		UPDATE dbo.[DefaultCommission]
		SET EndDate = @StartDate
		WHERE PersonId = @PersonId
			   AND [Type] = 1 -- Sales Commission
			   AND EndDate > @StartDate
		
		INSERT INTO dbo.Pay
					(Person, StartDate, EndDate, Amount, Timescale, TimesPaidPerMonth, Terms,
					 VacationDays, BonusAmount, BonusHoursToCollect,
					 DefaultHoursPerDay,SeniorityId,PracticeId)
			 VALUES (@PersonId, @StartDate, @EndDate, @Amount, @Timescale, @TimesPaidPerMonth, @Terms,
					 @VacationDays, @BonusAmount, ISNULL(@BonusHoursToCollect, dbo.GetHoursPerYear()),
					 @DefaultHoursPerDay,@SeniorityId,@PracticeId)

		IF @SalesCommissionFractionOfMargin IS NOT NULL
		INSERT INTO dbo.[DefaultCommission]
						(PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
			VALUES (@PersonId, @StartDate, '2029-12-31', @SalesCommissionFractionOfMargin, 1, NULL)

	END
	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))
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
	COMMIT TRAN

