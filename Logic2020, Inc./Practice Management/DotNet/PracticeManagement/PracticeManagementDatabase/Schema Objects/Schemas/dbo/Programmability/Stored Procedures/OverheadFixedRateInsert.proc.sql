-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-07-2008
-- Description:	Inserts the overhead.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadFixedRateInsert]
(
	@Description           NVARCHAR(255),
	@Rate                  DECIMAL(18,3),
	@StartDate             DATETIME,
	@EndDate               DATETIME,
	@RateType              INT,
	@Inactive              BIT,
	@IsCogs                BIT,
	@OverheadFixedRateId   INT OUT
)
AS
	SET NOCOUNT ON

	INSERT INTO dbo.OverheadFixedRate
	            (Description, Rate, StartDate, EndDate, RateType, Inactive, IsCogs)
	     VALUES (@Description, @Rate, @StartDate, @EndDate, @RateType, @Inactive, @IsCogs)

	SET @OverheadFixedRateId = SCOPE_IDENTITY()

