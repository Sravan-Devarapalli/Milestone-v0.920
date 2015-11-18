CREATE VIEW [dbo].[v_CurrentMSBadge]
	AS 
	With ByPerson
	AS
	(
		SELECT MAX(Id) Id, PersonId,BadgeStartDate 
		FROM dbo.BadgeHistoryForReports
		GROUP BY PersonId,BadgeStartDate
	),
	HistoryCTE
	AS
	(
		SELECT Id,PersonId,BadgeStartDate,ROW_NUMBER() OVER (PARTITION BY PersonId ORDER BY BadgeStartDate) RNum
		FROM ByPerson 
	),
	H1
	AS
	(
		SELECT C1.Id, C1.PersonId, CASE WHEN C1.RNum = 1 THEN NULL ELSE C1.BadgeStartDate END AS StartDate,C2.BadgeStartDate-1 AS EndDate,C1.RNum
		FROM HistoryCTE C1
		LEFT JOIN HistoryCTE C2 ON C2.PersonId = C1.PersonId AND C2.RNum = C1.RNum + 1
	)
	SELECT B.*,M.IsBlocked,M.BlockStartDate,M.BlockEndDate,M.DeactivatedDate,M.ExcludeInReports
	FROM H1
	INNER JOIN BadgeHistoryForReports B ON B.PersonId = H1.PersonId AND B.Id = H1.Id
	INNER JOIN dbo.MSBadge M ON M.PersonId = H1.PersonId
	WHERE (H1.StartDate IS NULL OR H1.StartDate <= CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE()))) AND (H1.EndDate IS NULL OR CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE())) <= H1.EndDate)


