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
	IF @prevStartDate= @Today
	BEGIN
		UPDATE [MinimumLoadFactorHistoryForUI]
		SET [W2-Hourly] = CASE WHEN @TimescaleId =1 THEN @Rate ELSE [W2-Hourly] END,
			[W2-Salary] = CASE WHEN @TimescaleId =2 THEN @Rate ELSE [W2-Salary] END,
			[1099] = CASE WHEN @TimescaleId =3 THEN @Rate ELSE [1099] END
		WHERE EndDate IS NULL
		
		SELECT	@W2_Hourly = [W2-Hourly]
				, @W2_Salary = [W2-Salary]
				, @1099 = [1099]
		FROM [MinimumLoadFactorHistoryForUI]
		WHERE [EndDate] IS NULL

		DECLARE  @PrevW2_Hourly DECIMAL, @PrevW2_Salary DECIMAL, @Prev1099 DECIMAL 
		SELECT @PrevW2_Hourly = [W2-Hourly] ,
				@PrevW2_Salary =[W2-Salary],
				@Prev1099 =  [1099]
		FROM [MinimumLoadFactorHistoryForUI]
		WHERE [EndDate] = @Today-1

		IF(@W2_Hourly = @PrevW2_Hourly AND @W2_Salary = @PrevW2_Salary AND @1099 = @Prev1099)
		BEGIN
			DELETE FROM [MinimumLoadFactorHistoryForUI]
			WHERE EndDate IS NULL 

			UPDATE [MinimumLoadFactorHistoryForUI]
			SET EndDate = NULL
			WHERE EndDate =  @Today-1
		END 

	END
	ELSE
	BEGIN
		UPDATE [MinimumLoadFactorHistoryForUI]
		SET [EndDate] =   @Today - 1 
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
END
