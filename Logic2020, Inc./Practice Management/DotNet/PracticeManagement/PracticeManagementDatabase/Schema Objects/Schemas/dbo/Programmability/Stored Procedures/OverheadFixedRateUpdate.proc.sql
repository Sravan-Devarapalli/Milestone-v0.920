-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-07-2008
-- Description:	Updates the overhead.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadFixedRateUpdate]
(
	@OverheadFixedRateId   INT,
	@Description           NVARCHAR(255),
	@Rate                  DECIMAL(18,3),
	@StartDate             DATETIME,
	@EndDate               DATETIME,
	@RateType              INT,
	@Inactive              BIT,
	@IsCogs                BIT
)
AS
	SET NOCOUNT ON

	UPDATE dbo.OverheadFixedRate
	   SET Description = @Description,
	       Rate = @Rate,
	       StartDate = @StartDate,
	       EndDate = @EndDate,
	       RateType = @RateType,
	       Inactive = @Inactive,
	       IsCogs = @IsCogs
	 WHERE OverheadFixedRateId = @OverheadFixedRateId

