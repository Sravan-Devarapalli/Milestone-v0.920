CREATE PROCEDURE [dbo].[SaveMinimumLoadFactorOverheadMultipliers](
	@Description NVARCHAR(255),
	@Inactive BIT,
	@W2HourlyMultiplier	DECIMAL,
	@W2SalaryMultiplier	DECIMAL,
	@Hourly1099Multiplier DECIMAL)
AS
	DECLARE @OverheadFixedRateId INT 
	SELECT @OverheadFixedRateId = OverheadFixedRateId
	FROM dbo.OverheadFixedRate 
	WHERE [Description] = @Description

	UPDATE dbo.OverheadFixedRate 
	SET Inactive = @Inactive
	WHERE OverheadFixedRateId = @OverheadFixedRateId 

	UPDATE dbo.OverheadFixedRateTimescale
	SET Rate =  CASE TimescaleId WHEN 1 THEN @W2HourlyMultiplier --W2-Hourly
								 WHEN 2 THEN @W2SalaryMultiplier --W2-Salary
								 ELSE @Hourly1099Multiplier END  --1099 Hourly
	WHERE OverheadFixedRateId = @OverheadFixedRateId

RETURN 0
