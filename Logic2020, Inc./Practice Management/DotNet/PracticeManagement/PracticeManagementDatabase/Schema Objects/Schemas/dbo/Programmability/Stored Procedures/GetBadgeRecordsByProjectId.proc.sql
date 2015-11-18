CREATE PROCEDURE [dbo].[GetBadgeRecordsByProjectId]
(
	@ProjectId	INT
)
AS
BEGIN

	SELECT	P.ProjectId,
			P.ProjectNumber,
			P.Name AS ProjectName,
			Per.PersonId,
			Per.FirstName,
			Per.LastName,
			MPE.BadgeStartDate,
			MPE.BadgeEndDate,
			MPE.IsBadgeException,
			M.MilestoneId,
			M.Description
	FROM dbo.MilestonePersonEntry MPE
	JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	WHERE	P.ProjectId = @ProjectId AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 0 AND MPE.BadgeStartDate IS NOT NULL AND MPE.BadgeEndDate IS NOT NULL
			
END

