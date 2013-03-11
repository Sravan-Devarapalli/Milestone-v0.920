-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-16-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects overheads for the timescales.
-- =============================================
CREATE VIEW [dbo].[v_OverheadFixedRateTimescale]
AS
	SELECT ofr.OverheadFixedRateId,
	    ofr.Description,
	    ofr.Rate,
	    ofr.StartDate,
	    ofr.EndDate,
	    ofr.Inactive,
	    ort.OverheadRateTypeId,
	    ort.Name AS OverheadRateTypeName,
	    ort.IsPercentage,
	    ort.HoursToCollect,
	    ofr.IsCogs,
	    ortt.TimescaleId
	FROM dbo.OverheadFixedRate AS ofr
	    INNER JOIN dbo.OverheadRateType AS ort ON ofr.RateType = ort.OverheadRateTypeId
		INNER JOIN dbo.OverheadFixedRateTimescale AS ortt ON ofr.OverheadFixedRateId = ortt.OverheadFixedRateId
	WHERE ofr.IsMinimumLoadFactor = 0 


