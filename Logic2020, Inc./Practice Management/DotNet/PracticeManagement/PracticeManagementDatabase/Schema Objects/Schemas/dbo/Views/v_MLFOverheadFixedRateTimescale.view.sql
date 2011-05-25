CREATE VIEW [dbo].[v_MLFOverheadFixedRateTimescale]
AS
	  SELECT o.Description,
			 MH.Rate,
			 t.HoursToCollect,
			 MH.StartDate,
			 MH.EndDate,
			 t.IsPercentage,
			 t.OverheadRateTypeId,
			 t.Name OverheadRateTypeName,
			 CASE t.OverheadRateTypeId
				WHEN 2 THEN o.Rate
				ELSE CAST(0 AS DECIMAL) 
				END AS BillRateMultiplier,
			MH.TimescaleId
	  FROM dbo.OverheadFixedRate AS o
	  JOIN dbo.OverheadRateType AS t ON o.RateType = t.OverheadRateTypeId
	  JOIN dbo.MinimumLoadFactorHistory AS MH  ON MH.OverheadFixedRateId = o.OverheadFixedRateId
	  WHERE o.IsMinimumLoadFactor = 1 AND o.Inactive = 0 AND MH.Rate > 0


