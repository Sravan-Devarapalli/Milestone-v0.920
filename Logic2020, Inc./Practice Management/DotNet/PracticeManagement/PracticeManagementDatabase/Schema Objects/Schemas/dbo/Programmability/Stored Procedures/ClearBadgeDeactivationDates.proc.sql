CREATE PROCEDURE [dbo].[ClearBadgeDeactivationDates]
(
	@PersonId	INT
)
AS
BEGIN

	IF EXISTS (
				SELECT 1 
				FROM dbo.MilestonePersonEntry MPE 
				JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				JOIN dbo.Milestone MS ON MS.MilestoneId = MP.MilestoneId
				JOIN dbo.MSBadge M ON M.PersonId = MP.PersonId
				JOIN dbo.Project P ON P.ProjectId = MS.ProjectId
				WHERE P.ProjectStatusId IN (1,2,3,4) AND MP.PersonId = @PersonId AND MPE.IsApproved = 1 AND M.DeactivatedDate IS NOT NULL AND (MPE.BadgeStartDate <= M.OrganicBreakEndDate AND M.DeactivatedDate <= MPE.BadgeEndDate)
			  )
	BEGIN

		UPDATE dbo.MSBadge
		SET DeactivatedDate = NULL,
			OrganicBreakStartDate = NULL,
			OrganicBreakEndDate = NULL
		WHERE PersonId = @PersonId

	END
	
END

