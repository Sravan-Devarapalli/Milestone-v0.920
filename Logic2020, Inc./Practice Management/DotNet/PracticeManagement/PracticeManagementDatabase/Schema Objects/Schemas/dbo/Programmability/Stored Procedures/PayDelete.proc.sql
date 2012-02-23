﻿CREATE PROCEDURE [dbo].[PayDelete]
	@PersonId INT, 
	@StartDate DATETIME
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN tran_PayDelete

		DECLARE @TempStartDate DATETIME,
				@TempEndDate DATETIME
		IF EXISTS(SELECT 1 FROM dbo.Person WHERE PersonId = @PersonId AND IsStrawman =1 )
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM dbo.Pay WHERE Person = @PersonId AND StartDate < @StartDate)
			BEGIN
				SELECT @TempEndDate = EndDate
				FROM  dbo.Pay
				WHERE Person = @PersonId AND StartDate = @StartDate

				DELETE FROM dbo.Pay
				WHERE Person = @PersonId AND StartDate = @StartDate

				UPDATE  dbo.Pay
				SET StartDate = @StartDate 
				WHERE StartDate = @TempEndDate AND Person = @PersonId
			END
			ELSE 
			BEGIN

				SELECT @TempEndDate = EndDate
				FROM  dbo.Pay
				WHERE Person = @PersonId AND StartDate = @StartDate

				DELETE FROM dbo.Pay
				WHERE Person = @PersonId AND StartDate = @StartDate

				UPDATE  dbo.Pay
				SET EndDate = @TempEndDate 
				WHERE EndDate = @StartDate AND Person = @PersonId
			END
		END
		ELSE IF EXISTS(SELECT 1 FROM dbo.Person WHERE PersonId = @PersonId AND IsStrawman =0 )
		BEGIN
			DECLARE @EndDate DATETIME , @Timescale INT ,@W2SalaryId INT
			SELECT @EndDate = enddate,@Timescale = Timescale FROM dbo.Pay WHERE Person = @PersonId AND StartDate = @StartDate
			SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'

			--Delete Holiday timeEntries if person is not w2salaried.
			IF (@Timescale = @W2SalaryId)
			BEGIN
				DECLARE @HolidayChargeCodeId INT
				SELECT @HolidayChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = dbo.GetHolidayTimeTypeId()

				DELETE TEH
				FROM dbo.TimeEntryHours TEH
				JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId 
				WHERE TE.ChargeCodeId  IN (@HolidayChargeCodeId ) 
						AND TE.PersonId = @PersonId 
						AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate

				DELETE TE
				FROM dbo.TimeEntry TE 
				WHERE TE.ChargeCodeId  IN (@HolidayChargeCodeId ) 
						AND TE.PersonId = @PersonId 
						AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate

			END
			
			DELETE FROM dbo.Pay
			WHERE Person = @PersonId AND StartDate = @StartDate

			COMMIT TRAN tran_PayDelete
		END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN tran_PayDelete
		
		DECLARE	 @ERROR_STATE	tinyint
		,@ERROR_SEVERITY		tinyint
		,@ERROR_MESSAGE		    nvarchar(2000)
		,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)

	END CATCH
END

