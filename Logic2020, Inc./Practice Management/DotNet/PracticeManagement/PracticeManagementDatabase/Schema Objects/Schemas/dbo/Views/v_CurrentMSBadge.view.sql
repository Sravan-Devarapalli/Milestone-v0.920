CREATE VIEW [dbo].[v_CurrentMSBadge]
	AS 
	WITH LatestDeactivationHistory
	AS
	(
	Select PersonId, MAX(OrganicBreakEndDate) as OrgBrkEndDate
	FROM BadgeDeactivationHisroty 
	GROUP BY PersonId
	)

	SELECT B.*,M.IsBlocked,M.BlockStartDate,M.BlockEndDate,M.DeactivatedDate,M.ExcludeInReports,M.ManageServiceContract
	FROM BadgeHistoryForReports B
	LEFT JOIN LatestDeactivationHistory D ON D.PersonId=B.PersonId
	LEFT JOIN dbo.MSBadge M ON M.PersonId = B.PersonId
	WHERE ((CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))>=B.BadgeStartDate AND CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))<= B.BadgeEndDate) OR 
	(CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))>=B.BreakStartDate AND CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))<= B.BreakEndDate) or (CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))<B.BadgeStartDate and CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))< B.BadgeEndDate))
	 AND (D.OrgBrkEndDate IS NULL OR B.BadgeStartDate>=D.OrgBrkEndDate)   

