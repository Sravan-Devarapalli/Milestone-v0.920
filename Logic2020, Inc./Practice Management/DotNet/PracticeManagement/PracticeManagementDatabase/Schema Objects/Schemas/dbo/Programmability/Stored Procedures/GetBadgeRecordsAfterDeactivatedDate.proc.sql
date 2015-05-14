CREATE PROCEDURE [dbo].[GetBadgeRecordsAfterDeactivatedDate]
(
	@PersonId			INT,
	@DeactivatedDate	DATETIME
)
AS
BEGIN 

	SELECT MPE.BadgeStartDate,MPE.BadgeEndDate
	FROM dbo.MilestonePersonEntry MPE
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	WHERE MP.PersonId = @PersonId AND MPE.IsBadgeRequired = 1 AND (@DeactivatedDate < MPE.BadgeEndDate) AND MPE.IsApproved = 1 AND P.ProjectStatusId IN (2,3)

END
