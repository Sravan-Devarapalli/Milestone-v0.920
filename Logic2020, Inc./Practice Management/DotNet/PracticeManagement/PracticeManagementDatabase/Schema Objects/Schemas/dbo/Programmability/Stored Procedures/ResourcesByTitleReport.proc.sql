﻿CREATE PROCEDURE [dbo].[ResourcesByTitleReport]
(
	@TitleIds			NVARCHAR(MAX),
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@Step				INT=7
)
AS
BEGIN

	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal DATETIME

	DECLARE @TitleIdsTable TABLE ( Ids INT )
	
	INSERT INTO @TitleIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@TitleIds)

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)

	;WIth Ranges
	AS
	(
		SELECT  c.MonthStartDate as StartDate,c.MonthEndDate  AS EndDate
		FROM dbo.Calendar c
		WHERE c.date BETWEEN @StartDateLocal and @EndDateLocal
		AND @Step = 30
		GROUP BY c.MonthStartDate,c.MonthEndDate  
		UNION ALL
		SELECT  c.date,c.date + 6
		FROM dbo.Calendar c
		WHERE c.date BETWEEN @StartDateLocal and @EndDateLocal
		AND DATEDIFF(day,@StartDateLocal,c.date) % 7 = 0
		AND @Step = 7
		UNION ALL
		SELECT  c.date,c.date
		FROM dbo.Calendar c
		WHERE c.date BETWEEN @StartDateLocal and @EndDateLocal
		AND @Step = 1
	),
	 BadgedOnProject
	 AS
	 (
		SELECT P.PersonId,P.TitleId,P.StartDate,P.EndDate
		FROM
	    (	
			SELECT MP.PersonId,P.TitleId,R.StartDate,R.EndDate
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN Ranges R ON MPE.BadgeStartDate <= R.EndDate AND R.StartDate <= MPE.BadgeEndDate
			INNER JOIN dbo.Person P ON P.PersonId = MP.PersonId 
			WHERE mpe.IsbadgeRequired = 1
			UNION ALL
			SELECT M.PersonId,P.TitleId,R.StartDate,R.EndDate
			FROM dbo.MSBadge M
			INNER JOIN Person P ON P.PersonId = M.PersonId 
			INNER JOIN Ranges R ON M.LastBadgeStartDate <= R.EndDate AND R.StartDate <= M.LastBadgeEndDate
			WHERE M.IsPreviousBadge = 1 
		)P
		GROUP BY P.PersonId,P.TitleId,P.StartDate,P.EndDate
	 ),
	 BadgedNotOnProject
	 AS
	 (
		SELECT M.PersonId,P.TitleId,R.StartDate,R.EndDate
		FROM v_CurrentMSBadge M 
		INNER JOIN Ranges R ON R.StartDate <= M.BadgeEndDate AND M.BadgeStartDate <= R.EndDate
		INNER JOIN dbo.Person P ON P.PersonId = M.PersonId
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = R.StartDate AND BP.PersonId = M.PersonId
		WHERE BP.PersonId IS NULL
		GROUP BY M.PersonId,P.TitleId,R.StartDate,R.EndDate 
	 ),
	  BadgedNotOnProjectCount
	 AS
	 (
		SELECT R.TitleId,R.StartDate,R.EndDate,COUNT(DISTINCT R.PersonId) AS BadgedNotOnProjectCount 
		FROM BadgedNotOnProject R
		INNER JOIN v_PersonHistory P ON P.PersonId = R.PersonId AND P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
		GROUP BY R.TitleId,R.StartDate,R.EndDate 
	 ),
	 BadgedProjectCount
	 AS
	 (
	    SELECT BP.TitleId,BP.StartDate,BP.EndDate,COUNT(DISTINCT BP.PersonId) AS BadgedOnProjectCount
		FROM BadgedOnProject BP
		INNER JOIN v_PersonHistory P ON P.PersonId = BP.PersonId AND P.HireDate <= BP.EndDate AND (P.TerminationDate IS NULL OR BP.StartDate <= p.TerminationDate)
		GROUP BY BP.TitleId,BP.StartDate,BP.EndDate
	 ),
	 ClockNotStarted
	 AS
	 (
	   SELECT M.PersonId,Per.TitleId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   INNER JOIN dbo.Person Per ON Per.PersonId = M.PersonId
	   INNER JOIN Ranges R ON R.EndDate < M.BadgeStartDate
	   GROUP BY M.PersonId,Per.TitleId,R.StartDate,R.EndDate
	 ),
	  BlockedPeople
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BlockEndDate AND M.BlockStartDate <= R.EndDate
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  InBreakPeriod
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BreakEndDate AND M.BreakStartDate <= R.EndDate
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  ClockNotStartedCount
	  AS
	  (
	   SELECT C.TitleId,C.StartDate,C.EndDate,COUNT(C.PersonId) AS ClockNotStartedCount
	   FROM ClockNotStarted C
	   INNER JOIN v_PersonHistory P ON P.PersonId = C.PersonId AND P.HireDate <= C.EndDate AND (P.TerminationDate IS NULL OR C.StartDate <= p.TerminationDate)
	   LEFT JOIN InBreakPeriod Brk ON Brk.StartDate = C.StartDate AND Brk.PersonId = C.PersonId
	   LEFT JOIN BlockedPeople B ON B.StartDate = C.StartDate AND B.PersonId = C.PersonId
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = C.StartDate AND BP.PersonId = C.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = C.StartDate AND BNP.PersonId = C.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND Brk.PersonId IS NULL
	   GROUP BY C.TitleId,C.StartDate,C.EndDate
	  )

	 SELECT T.TitleId,T.Title,R.StartDate,R.EndDate,ISNULL(BP.BadgedOnProjectCount,0) AS BadgedOnProjectCount,
			ISNULL(BNP.BadgedNotOnProjectCount,0) AS BadgedNotOnProjectCount,
			ISNULL(C.ClockNotStartedCount,0) AS ClockNotStartedCount
	 FROM Ranges R
	 CROSS JOIN dbo.Title T 
	 LEFT JOIN BadgedProjectCount BP ON BP.StartDate = R.StartDate AND BP.TitleId = T.TitleId
	 LEFT JOIN BadgedNotOnProjectCount BNP ON BNP.StartDate = R.StartDate AND BNP.TitleId = T.TitleId
	 LEFT JOIN ClockNotStartedCount C ON C.StartDate = R.StartDate AND C.TitleId = T.TitleId
	 WHERE T.TitleId IN (SELECT Ids FROM @TitleIdsTable)
	 ORDER BY R.StartDate,T.Title

END
