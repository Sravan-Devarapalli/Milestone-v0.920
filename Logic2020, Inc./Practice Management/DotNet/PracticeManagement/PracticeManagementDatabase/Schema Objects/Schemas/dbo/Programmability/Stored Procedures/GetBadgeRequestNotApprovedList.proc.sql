CREATE PROCEDURE [dbo].[GetBadgeRequestNotApprovedList]
AS
BEGIN

	SELECT	Per.LastName,
			Per.FirstName,
			P.ProjectId,
			P.Name AS ProjectName,
			P.ProjectNumber,
			MPE.BadgeStartDate,
			MPE.BadgeEndDate,
			MPE.BadgeRequestDate,
			P.ProjectStatusId,
			MB.BadgeEndDate AS ClockEndDate
	FROM dbo.MilestonePersonEntry MPE 
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
	WHERE MB.ExcludeInReports = 0 AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 0 AND P.ProjectStatusId IN (2,3)
	ORDER BY Per.LastName,Per.FirstName

END

