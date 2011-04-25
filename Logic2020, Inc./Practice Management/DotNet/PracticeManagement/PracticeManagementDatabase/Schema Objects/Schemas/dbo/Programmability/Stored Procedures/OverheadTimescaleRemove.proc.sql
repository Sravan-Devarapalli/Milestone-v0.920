--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Description:	Set the timescale for the overhead as non-applicable.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadTimescaleRemove]
(
	@OverheadFixedRateId   INT,
	@TimescaleId           INT
)
AS
	SET NOCOUNT ON

	DELETE
	  FROM dbo.OverheadFixedRateTimescale
	 WHERE OverheadFixedRateId = @OverheadFixedRateId
	   AND TimescaleId = @TimescaleId

