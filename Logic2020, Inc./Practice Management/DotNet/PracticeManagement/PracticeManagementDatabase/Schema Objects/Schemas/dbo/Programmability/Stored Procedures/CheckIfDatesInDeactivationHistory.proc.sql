CREATE PROCEDURE [dbo].[CheckIfDatesInDeactivationHistory]
(
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN

	DECLARE @OrganicBreaksExists BIT=0,
			@18MonthBreakExists	 BIT=0
			
	IF EXISTS(SELECT 1 FROM BadgeDeactivationHisroty WHERE PersonId = @PersonId AND ((OrganicBreakStartDate <= @EndDate AND @StartDate <= OrganicBreakEndDate) OR DeactivatedDate BETWEEN @StartDate AND @EndDate))
	BEGIN
		SET @OrganicBreaksExists = 1
	END
	IF EXISTS(SELECT 1 FROM v_CurrentMSBadge WHERE PersonId = @PersonId AND BreakStartDate <= @EndDate AND @StartDate <= BreakEndDate)
	BEGIN
		SET @18MonthBreakExists = 1
	END

	SELECT @OrganicBreaksExists AS OrganicBreaksExists, @18MonthBreakExists AS BadgeBreakExists
END
