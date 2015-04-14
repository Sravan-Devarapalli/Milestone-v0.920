CREATE PROCEDURE [dbo].[BadgedResourcesByTimeReport]
(
	@PayTypeIds	NVARCHAR(MAX)=NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@Step		INT=7
)
AS
BEGIN
	DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX)
	SET @PayTypeIdsLocal = @PayTypeIds

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

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
	 ActiveConsultantsRange
	 AS
	 (
		SELECT R.StartDate,R.EndDate,COUNT(*) as Count
		FROM Ranges R
		CROSS JOIN v_PersonHistory P 
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
		WHERE P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
			  AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		GROUP BY R.StartDate,R.EndDate
	 ),
	 BadgedOnProject
	 AS
	 (
		SELECT R.PersonId,R.StartDate,R.EndDate
		FROM
		(SELECT MP.PersonId,R.StartDate,R.EndDate
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		INNER JOIN Ranges R ON MPE.BadgeStartDate <= R.EndDate AND R.StartDate <= MPE.BadgeEndDate
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = MP.PersonId
		WHERE (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) AND mpe.IsbadgeRequired = 1 AND P.ProjectStatusId IN (1,2,3,4) 

		UNION ALL
		SELECT M.PersonId,R.StartDate,R.EndDate
		FROM dbo.MSBadge M
		INNER JOIN Ranges R ON M.LastBadgeStartDate <= R.EndDate AND R.StartDate <= M.LastBadgeEndDate
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
		WHERE M.IsPreviousBadge = 1 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) 
		) R
		GROUP BY R.PersonId,R.StartDate,R.EndDate
	 ),
	 BadgedNotOnProject
	 AS
	 (
		SELECT M.PersonId,R.StartDate,R.EndDate
		FROM v_CurrentMSBadge M 
		INNER JOIN Ranges R ON R.StartDate <= M.BadgeEndDate AND M.BadgeStartDate <= R.EndDate
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = R.StartDate AND BP.PersonId = M.PersonId
		WHERE BP.PersonId IS NULL AND
		(@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		GROUP BY M.PersonId,R.StartDate,R.EndDate 
	 ),
	  BadgedNotOnProjectCount
	 AS
	 (
		SELECT R.StartDate,R.EndDate,COUNT(DISTINCT R.PersonId) AS BadgedNotOnProjectCount 
		FROM BadgedNotOnProject R
		left JOIN v_PersonHistory P ON P.PersonId = R.PersonId AND P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
		where p.personid is not null
		GROUP BY R.StartDate,R.EndDate 
	 ),
	 BadgedProjectCount
	 AS
	 (
	    SELECT BP.StartDate,BP.EndDate,COUNT(DISTINCT BP.PersonId) AS BadgedOnProjectCount
		FROM BadgedOnProject BP
		LEFT JOIN v_PersonHistory P ON P.PersonId = BP.PersonId AND P.HireDate <= BP.EndDate AND (P.TerminationDate IS NULL OR BP.StartDate <= p.TerminationDate)
		GROUP BY BP.StartDate,BP.EndDate
	 ),
	 --ClockNotStarted
	 --AS
	 --(
	 --  SELECT M.PersonId,R.StartDate,R.EndDate
	 --  FROM v_CurrentMSBadge M 
	 --  JOIN Ranges R ON R.EndDate < M.BadgeStartDate
	 --  GROUP BY M.PersonId,R.StartDate,R.EndDate
	 --),
	  BlockedPeople
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BlockEndDate AND M.BlockStartDate <= R.EndDate
	   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
	   WHERE (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  InBreakPeriod
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BreakEndDate AND M.BreakStartDate <= R.EndDate
	   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
	   WHERE (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	 BlockedPeopleCount
	 AS
	 (
	   SELECT B.StartDate,B.EndDate,COUNT(B.PersonId) AS BlockedCount
	   FROM BlockedPeople B 
	   left JOIN v_PersonHistory P ON P.PersonId = B.PersonId AND P.HireDate <= B.EndDate AND (P.TerminationDate IS NULL OR B.StartDate <= p.TerminationDate)
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = B.StartDate AND BP.PersonId = B.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = B.StartDate AND BNP.PersonId = B.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL
	   GROUP BY B.StartDate,B.EndDate
	 ),
	  InBreakPeriodCount
	  AS
	  (
	  SELECT Brk.StartDate,Brk.EndDate,COUNT(Brk.PersonId) AS BlockedCount
	   FROM InBreakPeriod Brk 
	   left JOIN v_PersonHistory P ON P.PersonId = Brk.PersonId AND P.HireDate <= Brk.EndDate AND (P.TerminationDate IS NULL OR Brk.StartDate <= p.TerminationDate)
	   LEFT JOIN BlockedPeople B ON B.StartDate = Brk.StartDate AND B.PersonId = Brk.PersonId
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = Brk.StartDate AND BP.PersonId = Brk.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = Brk.StartDate AND BNP.PersonId = Brk.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL
	   GROUP BY Brk.StartDate,Brk.EndDate
	  )
	  --ClockNotStartedCount
	  --AS
	  --(
	  -- SELECT C.StartDate,C.EndDate,COUNT(C.PersonId) AS ClockNotStartedCount
	  -- FROM ClockNotStarted C
	  -- left JOIN v_PersonHistory P ON P.PersonId = C.PersonId AND P.HireDate <= C.EndDate AND (P.TerminationDate IS NULL OR C.StartDate <= p.TerminationDate)
	  -- LEFT JOIN InBreakPeriod Brk ON Brk.StartDate = C.StartDate AND Brk.PersonId = C.PersonId
	  -- LEFT JOIN BlockedPeople B ON B.StartDate = C.StartDate AND B.PersonId = C.PersonId
	  -- LEFT JOIN BadgedOnProject BP ON BP.StartDate = C.StartDate AND BP.PersonId = C.PersonId
	  -- LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = C.StartDate AND BNP.PersonId = C.PersonId
	  -- WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND Brk.PersonId IS NULL
	  -- GROUP BY C.StartDate,C.EndDate
	  --)

	 SELECT R.StartDate,R.EndDate,ISNULL(BP.BadgedOnProjectCount,0) AS BadgedOnProjectCount,
			ISNULL(BNP.BadgedNotOnProjectCount,0) AS BadgedNotOnProjectCount,
			--ISNULL(A.ClockNotStartedCount,0) AS ClockNotStartedCount,
			ISNULL(A.Count,0)-(ISNULL(BP.BadgedOnProjectCount,0) + ISNULL(BNP.BadgedNotOnProjectCount,0)+ ISNULL(B.BlockedCount,0) + ISNULL(Brk.BlockedCount,0)) AS ClockNotStartedCount,
			ISNULL(B.BlockedCount,0) AS BlockedCount,
			ISNULL(Brk.BlockedCount,0) AS InBreakPeriodCount
	 FROM Ranges R
	 LEFT JOIN ActiveConsultantsRange A ON A.StartDate = R.StartDate
	 LEFT JOIN BadgedProjectCount BP ON BP.StartDate = R.StartDate
	 LEFT JOIN BadgedNotOnProjectCount BNP ON BNP.StartDate = R.StartDate
	 --LEFT JOIN ClockNotStartedCount C ON C.StartDate = R.StartDate
	 LEFT JOIN BlockedPeopleCount B ON B.StartDate = R.StartDate
	 LEFT JOIN InBreakPeriodCount Brk ON Brk.StartDate = R.StartDate
	 ORDER BY R.StartDate
END
