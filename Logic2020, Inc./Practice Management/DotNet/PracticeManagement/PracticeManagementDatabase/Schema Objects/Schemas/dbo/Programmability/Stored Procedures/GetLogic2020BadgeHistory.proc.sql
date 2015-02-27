CREATE PROCEDURE [dbo].[GetLogic2020BadgeHistory]
(
	@PersonId		INT
)
AS
BEGIN

	SELECT P.ProjectId,
		   P.Name AS ProjectName,
		   P.ProjectNumber,
		   P.StartDate,
		   P.EndDate,
		   MPE.BadgeStartDate,
		   MPE.BadgeEndDate,
		   DATEDIFF(MM,MPE.BadgeStartDate,MPE.BadgeEndDate+1) BadgeDuration,
		   CONVERT(BIT,IsApproved) IsApproved
	FROM dbo.MilestonePersonEntry MPE
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	WHERE MPE.IsBadgeRequired = 1 AND MP.PersonId = @PersonId
	AND P.ClientId = 2 -- ClientId = 2 for MICROSOFT

	UNION ALL

	SELECT -1,
		   'Previous MS Badge History',
		   '',
		   M.LastBadgeStartDate,
		   M.LastBadgeEndDate,
		   M.LastBadgeStartDate,
		   M.LastBadgeEndDate,
		   DATEDIFF(MM,M.LastBadgeStartDate,M.LastBadgeEndDate+1),
		   CONVERT(BIT,0)
	FROM MSBadge M
	WHERE M.PersonId = @PersonId
	AND M.IsPreviousBadge = 1
	ORDER BY MPE.BadgeStartDate,P.ProjectNumber
END
