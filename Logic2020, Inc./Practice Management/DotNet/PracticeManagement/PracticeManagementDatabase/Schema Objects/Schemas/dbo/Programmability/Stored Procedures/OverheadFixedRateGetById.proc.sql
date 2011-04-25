-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-07-2008
-- Description:	Retrieves the overhead with its details.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadFixedRateGetById]
(
	@OverheadFixedRateId   INT
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
	 WHERE r.OverheadFixedRateId = @OverheadFixedRateId AND r.IsMinimumLoadFactor = 0

