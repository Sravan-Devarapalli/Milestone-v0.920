--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Description:	Set the timescale for the overhead as applicable.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadTimescaleSet]
(
	@OverheadFixedRateId   INT,
	@TimescaleId           INT
)
AS
	SET NOCOUNT ON

	IF NOT EXISTS (SELECT 1
	                 FROM dbo.OverheadFixedRateTimescale AS rt
	                WHERE rt.OverheadFixedRateId = @OverheadFixedRateId
	                  AND rt.TimescaleId = @TimescaleId)
	BEGIN
		INSERT INTO dbo.OverheadFixedRateTimescale
		            (OverheadFixedRateId, TimescaleId)
		     VALUES (@OverheadFixedRateId, @TimescaleId)
	END

