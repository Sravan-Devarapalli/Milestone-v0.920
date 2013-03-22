CREATE VIEW [dbo].[v_MLFOverheadFixedRateTimescale]
AS
	SELECT	MH.Rate,
			MH.StartDate,
			MH.EndDate,
			MH.TimescaleId
	FROM dbo.OverheadFixedRate AS o
	JOIN dbo.MinimumLoadFactorHistory AS MH  ON MH.OverheadFixedRateId = o.OverheadFixedRateId 
												AND o.IsMinimumLoadFactor = 1 
												AND o.Inactive = 0 
												AND MH.Rate > 0 
												AND o.RateType = 4 --Pay Rate Multiplier
