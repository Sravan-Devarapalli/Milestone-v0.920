CREATE PROCEDURE [dbo].[SaveBadgeDetailsByPersonId]
(
	@PersonId			INT,
	@IsBlocked			BIT,
	@BlockStartDate		DATETIME,
	@BlockEndDate		DATETIME,
	@IsPreviousBadge	BIT,
	@PreviousBadgeAlias	NVARCHAR(20),
	@LastBadgeStartDate	DATETIME,
	@LastBadgeEndDate	DATETIME,
	@IsException		BIT,
	@ExceptionStartDate	DATETIME,
	@ExceptionEndDate	DATETIME,
	@UpdatedBy			INT,
	@BadgeStartDate		DATETIME,
	@BadgeEndDate		DATETIME,
	@StartDateSource	NVARCHAR(30),
	@EndDateSource		NVARCHAR(30),
	@BreakStartDate		DATETIME,
	@BreakEndDate		DATETIME
)
AS
BEGIN

	UPDATE dbo.MSBadge 
	SET IsBlocked = @IsBlocked,
		BlockStartDate = @BlockStartDate,
		BlockEndDate = @BlockEndDate,
		IsPreviousBadge = @IsPreviousBadge,
		PreviousBadgeAlias = @PreviousBadgeAlias,
		LastBadgeStartDate = @LastBadgeStartDate,
		LastBadgeEndDate = @LastBadgeEndDate,
		IsException = @IsException,
		ExceptionStartDate = @ExceptionStartDate,
		ExceptionEndDate = @ExceptionEndDate,
		BadgeStartDate = @BadgeStartDate,
		BadgeEndDate = @BadgeEndDate,
		BreakStartDate = @BreakStartDate,
		BreakEndDate = @BreakEndDate,
		BadgeStartDateSource = @StartDateSource,
		BadgeEndDateSource = @EndDateSource
	WHERE PersonId = @PersonId

	IF(@StartDateSource = 'Manual Entry' OR @EndDateSource = 'Manual Entry')
	BEGIN

		UPDATE dbo.MSBadge 
		SET BadgeStartDate = @BadgeStartDate,
			BadgeEndDate = @BadgeEndDate,
			BreakStartDate = @BreakStartDate,
			BreakEndDate = @BreakEndDate,
			BadgeStartDateSource = @StartDateSource,
			BadgeEndDateSource = @EndDateSource
		WHERE PersonId = @PersonId

	END
  
  EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId,@UpdatedBy = @UpdatedBy

END
