CREATE PROCEDURE [dbo].[PayDelete]
	@PersonId INT, 
	@StartDate DATETIME
AS
BEGIN
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
END
