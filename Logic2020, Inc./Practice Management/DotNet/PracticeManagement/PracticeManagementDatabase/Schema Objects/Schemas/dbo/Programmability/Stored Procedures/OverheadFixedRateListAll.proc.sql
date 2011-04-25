-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-07-2008
-- Description:	Retrieves the list the overhead rates.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadFixedRateListAll]
(
	@ShowAll       BIT = 0
)
AS
	SET NOCOUNT ON

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
	 WHERE (r.Inactive = 0 OR @ShowAll = 1) AND IsMinimumLoadFactor = 0
	ORDER BY t.HoursToCollect, t.Name

