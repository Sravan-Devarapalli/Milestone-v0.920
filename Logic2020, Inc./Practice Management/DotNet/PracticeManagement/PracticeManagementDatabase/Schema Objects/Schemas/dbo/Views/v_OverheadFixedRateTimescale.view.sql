-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-16-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects overheads for the timescales.
-- =============================================
CREATE VIEW [dbo].[v_OverheadFixedRateTimescale]
AS
	SELECT o.OverheadFixedRateId,
	       o.Description,
	       o.Rate,
	       o.StartDate,
	       o.EndDate,
	       o.Inactive,
	       o.OverheadRateTypeId,
	       o.OverheadRateTypeName,
	       o.IsPercentage,
	       o.HoursToCollect,
	       o.IsCogs,
	       t.TimescaleId
	  FROM dbo.v_OverheadFixedRate AS o
	       INNER JOIN dbo.OverheadFixedRateTimescale AS t ON o.OverheadFixedRateId = t.OverheadFixedRateId

