CREATE VIEW [dbo].[v_CurrentMSBadge]
	AS 
	With HistoryCTE
	AS
	(
		SELECT *,DATEADD(mm,6,ProjectPlannedEndDate) as ReportRunMax,ROW_NUMBER() OVER(PARTITION BY PersonId ORDER BY badgestartdate) RNum
		FROM dbo.BadgeHistoryForReports
	),
	HistoryWithRunDates
	AS
	(
		SELECT c1.PersonId,c1.BadgeStartDate,c1.BadgeEndDate,c1.ProjectPlannedEndDate,c1.BreakStartDate,c1.BreakEndDate,CASE WHEN c1.rnum = 1 THEN NULL ELSE (SELECT ReportRunMax+1 FROM HistoryCTE c2 WHERE c2.PersonId = c1.PersonId and c2.RNum = c1.RNum-1) END AS ReportRunMin,
		       CASE WHEN c1.rnum =(SELECT MAX(C3.RNUM) FROM HistoryCTE c3 WHERE c3.PersonId = c1.PersonId GROUP BY c3.PersonId) THEN NULL ELSE ReportRunMax END AS ReportRunMax 
		FROM HistoryCTE c1
	)
	SELECT HR.*,M.BlockStartDate,M.BlockEndDate 
	FROM HistoryWithRunDates HR 
	INNER JOIN dbo.MSBadge M ON M.PersonId = HR.PersonId
	WHERE (HR.ReportRunMin is null or dbo.GettingPMTime(GETUTCDATE()) >= HR.ReportRunMin) and (HR.ReportRunMax is null or dbo.GettingPMTime(GETUTCDATE()) <= HR.ReportRunMax)
