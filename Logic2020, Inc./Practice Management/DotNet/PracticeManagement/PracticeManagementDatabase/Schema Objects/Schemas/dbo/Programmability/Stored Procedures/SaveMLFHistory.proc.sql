CREATE PROCEDURE [dbo].[SaveMLFHistory]
	@TimescaleId INT, 
	@Rate DECIMAL,
	@Today DATETIME
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @prevStartDate DATETIME, @W2_Hourly DECIMAL, @W2_Salary DECIMAL, @1099 DECIMAL 
	
	SELECT @prevStartDate = [StartDate]
			,@W2_Hourly = [W2-Hourly]
			, @W2_Salary = [W2-Salary]
			, @1099 = [1099]
	FROM [MinimumLoadFactorHistoryForUI]
	WHERE [EndDate] IS NULL
	
	UPDATE [MinimumLoadFactorHistoryForUI]
		SET [EndDate] = CASE WHEN (@Today  = @prevStartDate)  THEN @Today
						ELSE @Today - 1 end
	FROM [MinimumLoadFactorHistoryForUI]
	WHERE [EndDate] IS NULL
	
	IF (@TimescaleId = 1)
	BEGIN
		INSERT INTO [MinimumLoadFactorHistoryForUI] ([StartDate], [EndDate], [W2-Hourly], [W2-Salary], [1099])
		VALUES (@Today, NULL, @Rate, @W2_Salary, @1099)	
	END
	ELSE IF (@TimescaleId = 2)
	BEGIN
		INSERT INTO [MinimumLoadFactorHistoryForUI] ([StartDate], [EndDate], [W2-Hourly], [W2-Salary], [1099])
		VALUES (@Today, NULL, @W2_Hourly, @Rate, @1099)	
	END
	ELSE IF (@TimescaleId = 3)
	BEGIN
		INSERT INTO [MinimumLoadFactorHistoryForUI] ([StartDate], [EndDate], [W2-Hourly], [W2-Salary], [1099])
		VALUES (@Today, NULL, @W2_Hourly, @W2_Salary, @Rate)	
	END
END
