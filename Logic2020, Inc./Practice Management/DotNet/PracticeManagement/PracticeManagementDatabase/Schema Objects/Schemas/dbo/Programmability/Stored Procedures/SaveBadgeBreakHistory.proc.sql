CREATE PROCEDURE [dbo].[SaveBadgeBreakHistory]
(
	@PersonId	INT=NULL,
	@UpdatedBy  INT=NULL
)
AS
BEGIN

	DECLARE @Today DATETIME,
			@CurrentPMTime DATETIME

	SELECT @CurrentPMTime = dbo.GettingPMTime(GETUTCDATE())

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))


	INSERT INTO BadgeDeactivationHisroty(PersonId, DeactivatedDate, OrganicBreakStartDate, OrganicBreakEndDate,	ManualStartDate, ManualEndDate)
	SELECT M.PersonId, M.DeactivatedDate, M.OrganicBreakStartDate, M.OrganicBreakEndDate, M.ManualStartDate, M.ManualEndDate
	FROM MSBadge M
	WHERE M.PersonId = @PersonId
		  AND M.OrganicBreakEndDate <= (@Today-1)

	INSERT INTO dbo.BadgeHistory(PersonId,BadgeStartDate,BadgeEndDate,BadgeStartDateSource,BadgeEndDateSource,BreakStartDate,BreakEndDate,ProjectPlannedEndDate,ProjectPlannedEndDateSource,ModifiedDate,ModifiedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports)
		SELECT PersonId,NULL,NULL,'Badge Deactivation','Badge Deactivation',OrganicBreakStartDate,OrganicBreakEndDate,null,null,@CurrentPMTime,@UpdatedBy,DeactivatedDate,OrganicBreakStartDate,OrganicBreakEndDate,ExcludeInReports
		FROM dbo.MSBadge  M
		WHERE M.PersonId = @PersonId
			  AND M.OrganicBreakEndDate <= (@Today-1)


	UPDATE M
	SET M.DeactivatedDate		=	NULL,
		M.OrganicBreakStartDate =	NULL,
		M.OrganicBreakEndDate	=	NULL,
		M.ManualStartDate		=	NULL, 
		M.ManualEndDate			=	NULL
	FROM MSBadge M
	WHERE M.PersonId = @PersonId 
		  AND M.OrganicBreakEndDate <= (@Today-1)

END
