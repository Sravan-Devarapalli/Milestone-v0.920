CREATE PROCEDURE [dbo].[ResourcesByPracticeReport]
(
	@PayTypeIds			NVARCHAR(MAX)=NULL,
	@PersonStatusIds	NVARCHAR(MAX),
	@Practices			NVARCHAR(MAX),
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@Step				INT=7
)
AS
BEGIN

	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal DATETIME

	DECLARE @PracticeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),
			@PersonStatusIdsLocal NVARCHAR(MAX)
	SET @PayTypeIdsLocal = @PayTypeIds
	SET @PersonStatusIdsLocal = @PersonStatusIds

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

	INSERT INTO @PersonStatusIdsTable(Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsLocal)

	INSERT INTO @PracticeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@Practices)

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
		SELECT P.PersonId,P.DefaultPractice,P.StartDate,P.EndDate
		FROM
		(
		SELECT MP.PersonId,P.DefaultPractice,R.StartDate,R.EndDate
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN Ranges R ON MPE.BadgeStartDate <= R.EndDate AND R.StartDate <= MPE.BadgeEndDate
		INNER JOIN dbo.Person P ON P.PersonId = MP.PersonId 
		INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
		WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1
				AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		UNION ALL

		SELECT M.PersonId,P.DefaultPractice,R.StartDate,R.EndDate
		FROM dbo.MSBadge M
		INNER JOIN v_CurrentMSBadge MB ON M.PersonId = MB.PersonId
		INNER JOIN Person P ON P.PersonId = M.PersonId 
		INNER JOIN Ranges R ON M.LastBadgeStartDate <= R.EndDate AND R.StartDate <= M.LastBadgeEndDate
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
		WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
			  AND (M.LastBadgeStartDate <= MB.BadgeEndDate AND MB.BadgeStartDate <= M.LastBadgeEndDate)
		) P
		GROUP BY P.PersonId,P.DefaultPractice,P.StartDate,P.EndDate
	 ),
	 BadgedNotOnProject
	 AS
	 (
		SELECT M.PersonId,P.DefaultPractice,R.StartDate,R.EndDate
		FROM v_CurrentMSBadge M 
		INNER JOIN Ranges R ON R.StartDate <= M.BadgeEndDate AND M.BadgeStartDate <= R.EndDate
		INNER JOIN dbo.Person P ON P.PersonId = M.PersonId
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = R.StartDate AND BP.PersonId = M.PersonId
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
		WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		GROUP BY M.PersonId,P.DefaultPractice,R.StartDate,R.EndDate 
	 ),
	  BadgedNotOnProjectCount
	 AS
	 (
		SELECT R.DefaultPractice,R.StartDate,R.EndDate,COUNT(DISTINCT R.PersonId) AS BadgedNotOnProjectCount 
		FROM BadgedNotOnProject R
		INNER JOIN v_PersonHistory P ON P.PersonId = R.PersonId AND P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
		WHERE P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
		GROUP BY R.DefaultPractice,R.StartDate,R.EndDate 
	 ),
	 BadgedProjectCount
	 AS
	 (
	    SELECT BP.DefaultPractice,BP.StartDate,BP.EndDate,COUNT(DISTINCT BP.PersonId) AS BadgedOnProjectCount
		FROM BadgedOnProject BP
		INNER JOIN v_PersonHistory P ON P.PersonId = BP.PersonId AND P.HireDate <= BP.EndDate AND (P.TerminationDate IS NULL OR BP.StartDate <= p.TerminationDate)
		WHERE P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
		GROUP BY BP.DefaultPractice,BP.StartDate,BP.EndDate
	 ),
	 ClockNotStarted
	 AS
	 (
	   SELECT M.PersonId,Per.DefaultPractice,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   INNER JOIN dbo.Person Per ON Per.PersonId = M.PersonId
	   INNER JOIN Ranges R ON R.EndDate < M.BadgeStartDate
	   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,Per.DefaultPractice,R.StartDate,R.EndDate
	 ),
	  BlockedPeople
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BlockEndDate AND M.BlockStartDate <= R.EndDate
	   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  InBreakPeriod
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BreakEndDate AND M.BreakStartDate <= R.EndDate
	   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  ClockNotStartedCount
	  AS
	  (
	   SELECT C.DefaultPractice,C.StartDate,C.EndDate,COUNT(C.PersonId) AS ClockNotStartedCount
	   FROM ClockNotStarted C
	   INNER JOIN v_PersonHistory P ON P.PersonId = C.PersonId AND P.HireDate <= C.EndDate AND (P.TerminationDate IS NULL OR C.StartDate <= p.TerminationDate)
	   LEFT JOIN InBreakPeriod Brk ON Brk.StartDate = C.StartDate AND Brk.PersonId = C.PersonId
	   LEFT JOIN BlockedPeople B ON B.StartDate = C.StartDate AND B.PersonId = C.PersonId
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = C.StartDate AND BP.PersonId = C.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = C.StartDate AND BNP.PersonId = C.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND Brk.PersonId IS NULL
			 AND P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
	   GROUP BY C.DefaultPractice,C.StartDate,C.EndDate
	  )

	 SELECT Pr.PracticeId,Pr.Name AS PracticeName,R.StartDate,R.EndDate,ISNULL(BP.BadgedOnProjectCount,0) AS BadgedOnProjectCount,
			ISNULL(BNP.BadgedNotOnProjectCount,0) AS BadgedNotOnProjectCount,
			ISNULL(C.ClockNotStartedCount,0) AS ClockNotStartedCount
	 FROM Ranges R
	 CROSS JOIN dbo.Practice Pr 
	 LEFT JOIN BadgedProjectCount BP ON BP.StartDate = R.StartDate AND BP.DefaultPractice = Pr.PracticeId
	 LEFT JOIN BadgedNotOnProjectCount BNP ON BNP.StartDate = R.StartDate AND BNP.DefaultPractice = Pr.PracticeId
	 LEFT JOIN ClockNotStartedCount C ON C.StartDate = R.StartDate AND C.DefaultPractice = Pr.PracticeId
	 WHERE Pr.PracticeId IN (SELECT Ids FROM @PracticeIdsTable)
	 ORDER BY R.StartDate,Pr.Name
END

