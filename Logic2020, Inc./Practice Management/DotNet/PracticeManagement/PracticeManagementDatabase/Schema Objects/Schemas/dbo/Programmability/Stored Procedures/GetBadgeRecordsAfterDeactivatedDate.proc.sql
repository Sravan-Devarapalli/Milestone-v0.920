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
	WHERE MP.PersonId = @PersonId AND MPE.IsBadgeRequired = 1 AND (@DeactivatedDate < MPE.BadgeEndDate) AND MPE.IsApproved = 1

END
