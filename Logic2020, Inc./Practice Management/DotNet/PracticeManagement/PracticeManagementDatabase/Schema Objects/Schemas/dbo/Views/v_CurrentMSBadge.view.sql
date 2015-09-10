CREATE VIEW [dbo].[v_CurrentMSBadge]
	AS 
	--With HistoryCTE
	--AS
	--(
	--	SELECT *,DATEADD(mm,6,ProjectPlannedEndDate) as ReportRunMax,ROW_NUMBER() OVER(PARTITION BY PersonId ORDER BY badgestartdate) RNum
	--	FROM dbo.BadgeHistoryForReports
	--),
	--HistoryWithRunDates
	--AS
	--(
	--	SELECT c1.PersonId,c1.BadgeStartDate,c1.BadgeEndDate,c1.ProjectPlannedEndDate,c1.BreakStartDate,c1.BreakEndDate,CASE WHEN c1.rnum = 1 THEN NULL ELSE (SELECT ReportRunMax+1 FROM HistoryCTE c2 WHERE c2.PersonId = c1.PersonId and c2.RNum = c1.RNum-1) END AS ReportRunMin,
	--	       CASE WHEN c1.rnum =(SELECT MAX(C3.RNUM) FROM HistoryCTE c3 WHERE c3.PersonId = c1.PersonId GROUP BY c3.PersonId) THEN NULL ELSE ReportRunMax END AS ReportRunMax 
	--	FROM HistoryCTE c1
	--)
	--SELECT HR.*,M.BlockStartDate,M.BlockEndDate,M.DeactivatedDate,M.ExcludeInReports
	--FROM HistoryWithRunDates HR 
	--INNER JOIN dbo.MSBadge M ON M.PersonId = HR.PersonId
	--WHERE (HR.ReportRunMin is null or dbo.GettingPMTime(GETUTCDATE()) >= HR.ReportRunMin) and (HR.ReportRunMax is null or dbo.GettingPMTime(GETUTCDATE()) <= HR.ReportRunMax)
	With HistoryCTE
	AS
	(
		SELECT Id,PersonId,BadgeStartDate,ROW_NUMBER() OVER (PARTITION BY PersonId ORDER BY BadgeStartDate) RNum
		FROM dbo.BadgeHistoryForReports 
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


