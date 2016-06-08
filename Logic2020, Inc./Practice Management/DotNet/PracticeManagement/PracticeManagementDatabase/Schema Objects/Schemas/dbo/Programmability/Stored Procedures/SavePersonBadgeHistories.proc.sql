CREATE PROCEDURE [dbo].[SavePersonBadgeHistories]
(
	@PersonId	INT,
	@UpdatedBy	INT
)
AS
BEGIN

	DECLARE @BlockStartDate DATETIME,
			@BlockEndDate	DATETIME,
			@OverrideStartDate DATETIME,
			@OverrideEndDate DATETIME,
			@CurrentBlockStartDate DATETIME,
			@CurrentBlockEndDate DATETIME,
			@CurrentOverrideStartDate DATETIME,
			@CurrentOverrideEndDate DATETIME,
			@CurrentPMTime DATETIME,
			@CurrentDeactivatedDate	DATETIME,

			@DeactivatedDate DATETIME,
			@BadgeStartDate DATETIME,
			@BadgeEndDate DATETIME,
			@PlannedEndDate	DATETIME,
			@CurrentBadgeStartDate DATETIME,
			@CurrentBadgeEndDate DATETIME,
			@CurrentPlannedEndDate DATETIME,
			@MinDate	DATETIME = '19000101'

	SELECT @CurrentPMTime = dbo.GettingPMTime(GETUTCDATE())
	SELECT @BlockStartDate = BlockStartDate, @BlockEndDate = BlockEndDate
	FROM dbo.BlockHistory WHERE Id =(SELECT MAX(Id) FROM dbo.BlockHistory WHERE PersonId = @PersonId GROUP BY PersonId)

	SELECT @OverrideStartDate = OverrideStartDate, @OverrideEndDate = OverrideEndDate
	FROM dbo.OverrideExceptionHistory WHERE Id =(SELECT MAX(Id) FROM dbo.OverrideExceptionHistory WHERE PersonId = @PersonId GROUP BY PersonId)

	SELECT @BadgeStartDate = BH.BadgeStartDate, @BadgeEndDate = BH.BadgeEndDate,@PlannedEndDate = BH.ProjectPlannedEndDate, @DeactivatedDate = BH.DeactivatedDate
	FROM dbo.BadgeHistory BH WHERE Id =(SELECT MAX(Id) FROM dbo.BadgeHistory WHERE PersonId = @PersonId AND BadgeStartDate IS NOT NULL GROUP BY PersonId) 

	SELECT	@CurrentBlockStartDate = BlockStartDate,
			@CurrentBlockEndDate = BlockEndDate,
			@CurrentOverrideStartDate = ExceptionStartDate,
			@CurrentOverrideEndDate = ExceptionEndDate,
			@CurrentBadgeStartDate = BadgeStartDate,
			@CurrentBadgeEndDate = BadgeEndDate,
			@CurrentPlannedEndDate = PlannedEndDate,
			@CurrentDeactivatedDate = DeactivatedDate
	FROM dbo.MSBadge WHERE PersonId = @PersonId

	IF((ISNULL(@BlockStartDate,@MinDate) <> ISNULL(@CurrentBlockStartDate,@MinDate)) OR (ISNULL(@BlockEndDate,@MinDate) <> ISNULL(@CurrentBlockEndDate,@MinDate)))
	BEGIN

		INSERT INTO dbo.BlockHistory(PersonId,BlockStartDate,BlockEndDate,ModifiedDate,ModifiedBy)
		SELECT @PersonId,@CurrentBlockStartDate,@CurrentBlockEndDate,@CurrentPMTime,@UpdatedBy

	END

	IF((ISNULL(@OverrideStartDate,@MinDate) <> ISNULL(@CurrentOverrideStartDate,@MinDate)) OR (ISNULL(@OverrideEndDate,@MinDate) <> ISNULL(@CurrentOverrideEndDate,@MinDate)))
	BEGIN

		INSERT INTO dbo.OverrideExceptionHistory(PersonId,OverrideStartDate,OverrideEndDate,ModifiedDate,ModifiedBy)
		SELECT @PersonId,@CurrentOverrideStartDate,@CurrentOverrideEndDate,@CurrentPMTime,@UpdatedBy

	END

	IF ((ISNULL(@DeactivatedDate,@MinDate) <> ISNULL(@CurrentDeactivatedDate,@MinDate)) AND @CurrentDeactivatedDate IS NOT NULL)
	BEGIN
	INSERT INTO dbo.BadgeHistory(PersonId,BadgeStartDate,BadgeEndDate,BadgeStartDateSource,BadgeEndDateSource,BreakStartDate,BreakEndDate,ProjectPlannedEndDate,ProjectPlannedEndDateSource,ModifiedDate,ModifiedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports)
		SELECT PersonId,NULL,NULL,'Badge Deactivation','Badge Deactivation',OrganicBreakStartDate,OrganicBreakEndDate,NULL,NULL,@CurrentPMTime,@UpdatedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports
		FROM dbo.MSBadge 
		WHERE PersonId = @PersonId
	END

	ELSE IF((ISNULL(@BadgeStartDate,@MinDate) <> ISNULL(@CurrentBadgeStartDate,@MinDate)) OR (ISNULL(@BadgeEndDate,@MinDate) <> ISNULL(@CurrentBadgeEndDate,@MinDate)) OR (ISNULL(@PlannedEndDate,@MinDate) <> ISNULL(@CurrentPlannedEndDate,@MinDate)))
	BEGIN

		INSERT INTO dbo.BadgeHistory(PersonId,BadgeStartDate,BadgeEndDate,BadgeStartDateSource,BadgeEndDateSource,BreakStartDate,BreakEndDate,ProjectPlannedEndDate,ProjectPlannedEndDateSource,ModifiedDate,ModifiedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports)
		SELECT PersonId,BadgeStartDate,BadgeEndDate,BadgeStartDateSource,BadgeEndDateSource,BreakStartDate,BreakEndDate,PlannedEndDate,PlannedEndDateSource,@CurrentPMTime,@UpdatedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports
		FROM dbo.MSBadge 
		WHERE PersonId = @PersonId

	END

END

