-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 7-30-2008
-- Description:	Lists an overhead for the specified timescale
-- =============================================
CREATE PROCEDURE [dbo].[PersonOverheadByTimescale]
(
	@TimescaleId   INT,
	@EffectiveDate DATETIME = null
)
AS
	SET NOCOUNT ON

	DECLARE @FutureDate DATETIME
 	SELECT @FutureDate = dbo.GetFutureDate()

    IF @EffectiveDate IS NULL
	BEGIN
		SELECT @EffectiveDate = dbo.Today()
	END

	SELECT o.Description,
	       o.Rate,
	       t.HoursToCollect,
	       o.StartDate,
	       o.EndDate,
	       t.IsPercentage,
	       t.OverheadRateTypeId,
	       t.Name OverheadRateTypeName,
	       CASE t.OverheadRateTypeId
	           WHEN 2
	           THEN o.Rate
	           ELSE CAST(0 AS DECIMAL)
	       END AS BillRateMultiplier,
		   o.IsMinimumLoadFactor
	  FROM dbo.OverheadFixedRate AS o
	  JOIN dbo.OverheadRateType AS t ON o.RateType = t.OverheadRateTypeId
	  JOIN dbo.OverheadFixedRateTimescale AS ot  ON ot.OverheadFixedRateId = o.OverheadFixedRateId
	 WHERE  ot.TimescaleId = @TimescaleId AND o.IsMinimumLoadFactor = 0
			AND o.StartDate <= @EffectiveDate AND ISNULL(o.EndDate, @FutureDate) > @EffectiveDate
			AND o.Inactive = 0

	UNION ALL
	SELECT o.Description,
			MH.Rate,
			t.HoursToCollect,
			MH.StartDate,
			MH.EndDate,
			t.IsPercentage,
				   t.OverheadRateTypeId,
				   t.Name OverheadRateTypeName,
				   CASE t.OverheadRateTypeId
					   WHEN 2
					   THEN o.Rate
					   ELSE CAST(0 AS DECIMAL)
				   END AS BillRateMultiplier,
			o.IsMinimumLoadFactor
	FROM dbo.OverheadFixedRate AS o
	JOIN dbo.OverheadRateType AS t ON o.RateType = t.OverheadRateTypeId
	JOIN dbo.MinimumLoadFactorHistory MH ON MH.OverheadFixedRateId = o.OverheadFixedRateId 
	WHERE o.IsMinimumLoadFactor = 1
			AND o.Inactive = 0
			AND  MH.TimescaleId = @TimescaleId
			AND @EffectiveDate >= MH.StartDate AND (@EffectiveDate<= MH.EndDate OR MH.EndDate  IS NULL)

