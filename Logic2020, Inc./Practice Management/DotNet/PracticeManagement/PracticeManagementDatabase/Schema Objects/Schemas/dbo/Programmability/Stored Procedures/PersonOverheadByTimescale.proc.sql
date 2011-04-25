-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 7-30-2008
-- Description:	Lists an overhead for the specified timescale
-- =============================================
CREATE PROCEDURE [dbo].[PersonOverheadByTimescale]
(
	@TimescaleId   INT
)
AS
	SET NOCOUNT ON

	DECLARE @today DATETIME
	SET @today = dbo.Today()

	SELECT o.Description,
	       CASE WHEN o.IsMinimumLoadFactor = 1 THEN ot.Rate
		   ELSE  o.Rate END Rate,
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
	 WHERE  ot.TimescaleId = @TimescaleId
			AND o.StartDate <= @today AND ISNULL(o.EndDate, dbo.GetFutureDate()) > @today
			AND o.Inactive = 0

