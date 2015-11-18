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
	@BreakEndDate		DATETIME,
	@DeactivatedDate	DATETIME,
	@OrganicBreakStart	DATETIME,
	@OrganicBreakEnd	DATETIME,
	@ExcludeFromReports	BIT,
	@ManageServiceContract	BIT
)
AS
BEGIN

	DECLARE @PreviousIsManual BIT
	SELECT @PreviousIsManual = CASE WHEN M.BadgeStartDateSource = 'Manual Entry' THEN 1 ELSE 0 END
	FROM dbo.MSBadge M 
	WHERE M.PersonId = @PersonId

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
		BadgeEndDateSource = @EndDateSource,
		DeactivatedDate = @DeactivatedDate,
		OrganicBreakStartDate = @OrganicBreakStart,
		OrganicBreakEndDate = @OrganicBreakEnd,
		ExcludeInReports = @ExcludeFromReports,
		ManageServiceContract = @ManageServiceContract
	WHERE PersonId = @PersonId

	IF(@StartDateSource = 'Manual Entry' OR @EndDateSource = 'Manual Entry')
	BEGIN

		UPDATE dbo.MSBadge 
		SET ManualStartDate = @BadgeStartDate,
			ManualEndDate = @BadgeEndDate,
			ManualBreakStart = @BreakStartDate,
			ManualBreakEnd = @BreakEndDate,
			BadgeStartDateSource = @StartDateSource,
			BadgeEndDateSource = @EndDateSource
		WHERE PersonId = @PersonId

	END
 
	IF(@StartDateSource ='Available Now' AND @PreviousIsManual = 1)
	BEGIN
		UPDATE dbo.MSBadge 
		SET ManualStartDate = @BadgeStartDate,
			ManualEndDate = @BadgeEndDate,
			ManualBreakStart = @BreakStartDate,
			ManualBreakEnd = @BreakEndDate,
			BadgeStartDateSource = @StartDateSource,
			BadgeEndDateSource = @EndDateSource
		WHERE PersonId = @PersonId
	END

  	UPDATE MPE
	SET MPE.BadgeEndDate = @DeactivatedDate-1
	FROM dbo.MilestonePersonEntry MPE
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	WHERE MP.PersonId = @PersonId AND MPE.IsBadgeRequired = 1 AND (@DeactivatedDate-1 BETWEEN MPE.BadgeStartDate AND MPE.BadgeEndDate) AND MPE.IsApproved = 1
  
  EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId,@UpdatedBy = @UpdatedBy

END

