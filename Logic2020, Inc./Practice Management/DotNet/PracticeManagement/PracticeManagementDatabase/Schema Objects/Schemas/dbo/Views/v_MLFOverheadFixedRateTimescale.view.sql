CREATE VIEW [dbo].[v_MLFOverheadFixedRateTimescale]
AS
	  SELECT o.Description,
			 ot.Rate,
			 t.HoursToCollect,
			 o.StartDate,
			 o.EndDate,
			 t.IsPercentage,
			 t.OverheadRateTypeId,
			 t.Name OverheadRateTypeName,
			 CASE t.OverheadRateTypeId
				WHEN 2 THEN o.Rate
				ELSE CAST(0 AS DECIMAL) 
				END AS BillRateMultiplier,
			ot.TimescaleId
	  FROM dbo.OverheadFixedRate AS o
	  JOIN dbo.OverheadRateType AS t ON o.RateType = t.OverheadRateTypeId
	  JOIN dbo.OverheadFixedRateTimescale AS ot  ON ot.OverheadFixedRateId = o.OverheadFixedRateId
	  WHERE o.IsMinimumLoadFactor = 1 AND o.Inactive = 0 AND ot.Rate > 0


