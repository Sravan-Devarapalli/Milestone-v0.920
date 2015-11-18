CREATE PROCEDURE [dbo].[GetBadgeRequestNotApprovedList]
AS
BEGIN

	SELECT	Per.LastName,
			ISNULL(Per.PreferredFirstName,Per.FirstName) AS FirstName,
			P.ProjectId,
			P.Name AS ProjectName,
			P.ProjectNumber,
			MPE.BadgeStartDate,
			MPE.BadgeEndDate,
			MPE.BadgeRequestDate,
			P.ProjectStatusId,
			MB.BadgeEndDate AS ClockEndDate,
			MPE.Requester AS RequesterId,
			R.LastName+', '+ISNULL(R.PreferredFirstName,R.FirstName) AS Requester,
			Per.TitleId,
			Per.Title,
			M.MilestoneId,
			M.Description
	FROM dbo.MilestonePersonEntry MPE 
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	INNER JOIN v_Person Per ON Per.PersonId = MP.PersonId
	INNER JOIN v_CurrentMSBadge MB ON MB.PersonId = MP.PersonId
	LEFT JOIN dbo.Person R ON R.PersonId = MPE.Requester
	WHERE ISNULL(MB.ExcludeInReports,0) = 0 AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 0 AND P.ProjectStatusId IN (2,3)
	ORDER BY Per.LastName,ISNULL(Per.PreferredFirstName,Per.FirstName)

END

