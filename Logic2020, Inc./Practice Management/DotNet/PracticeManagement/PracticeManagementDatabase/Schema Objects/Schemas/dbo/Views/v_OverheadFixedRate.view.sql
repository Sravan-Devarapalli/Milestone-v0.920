-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-07-2008
-- Description:	Lists overhead rates with their details
-- =============================================
CREATE VIEW [dbo].[v_OverheadFixedRate]
AS
	SELECT r.OverheadFixedRateId,
	       r.Description,
	       r.Rate,
	       r.StartDate,
	       r.EndDate,
	       r.Inactive,
	       r.RateType AS OverheadRateTypeId,
	       t.Name AS OverheadRateTypeName,
	       t.IsPercentage,
	       t.HoursToCollect,
	       r.IsCogs
	  FROM dbo.OverheadFixedRate AS r
	       INNER JOIN dbo.OverheadRateType AS t ON r.RateType = t.OverheadRateTypeId
	  WHERE r.IsMinimumLoadFactor = 0

