CREATE PROCEDURE [dbo].[GetMinimumLoadFactorMultipliers]
	@Description NVARCHAR(225),
	@Inactive	 BIT  OUTPUT
AS
	SELECT MH.TimescaleId, MH.Rate
	FROM dbo.OverheadFixedRate O
	JOIN dbo.MinimumLoadFactorHistory AS MH  ON MH.OverheadFixedRateId = o.OverheadFixedRateId AND MH.EndDate IS NULL
	WHERE O.IsMinimumLoadFactor = 1 AND O.Description = @Description

	SELECT @Inactive = Inactive
	FROM dbo.OverheadFixedRate WHERE IsMinimumLoadFactor = 1 AND [Description] = @Description
	IF @Inactive IS NULL
	SELECT @Inactive = 0
RETURN 0
