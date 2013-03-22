CREATE VIEW [dbo].[v_OverheadFixedRateTimescale]
AS
	SELECT	ofr.Rate,
			ofr.StartDate,
			ofr.EndDate,
			ofr.RateType AS OverheadRateTypeId,
			ortt.TimescaleId
	FROM dbo.OverheadFixedRate AS ofr
		INNER JOIN dbo.OverheadFixedRateTimescale AS ortt ON ortt.OverheadFixedRateId = ofr.OverheadFixedRateId AND ofr.IsMinimumLoadFactor = 0 AND ofr.Inactive = 0

