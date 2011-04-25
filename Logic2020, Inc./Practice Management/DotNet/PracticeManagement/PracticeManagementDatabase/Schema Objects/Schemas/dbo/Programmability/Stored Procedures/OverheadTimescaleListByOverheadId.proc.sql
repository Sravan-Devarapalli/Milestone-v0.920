--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Description:	Lists overhead timescales
-- =============================================
CREATE PROCEDURE [dbo].[OverheadTimescaleListByOverheadId]
(
	@OverheadFixedRateId   INT
)
AS
	SET NOCOUNT ON

	SELECT r.OverheadFixedRateId,
	       t.TimescaleId,
	       CAST(CASE WHEN o.OverheadFixedRateId IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsSet
	  FROM dbo.OverheadFixedRate AS r
	       CROSS JOIN dbo.Timescale AS t
	       LEFT JOIN dbo.OverheadFixedRateTimescale o
	           ON o.OverheadFixedRateId = r.OverheadFixedRateId AND o.TimescaleId = t.TimescaleId
	 WHERE r.OverheadFixedRateId = @OverheadFixedRateId

