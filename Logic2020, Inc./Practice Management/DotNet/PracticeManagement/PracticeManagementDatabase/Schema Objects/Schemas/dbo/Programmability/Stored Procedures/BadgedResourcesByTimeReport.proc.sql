CREATE PROCEDURE [dbo].[BadgedResourcesByTimeReport]
(
	@PayTypeIds	NVARCHAR(MAX)=NULL,
	@PersonStatusIds NVARCHAR(MAX),
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@Step		INT=7
)
AS
BEGIN
	DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )
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
		SELECT R.StartDate,R.EndDate,COUNT(DISTINCT P.PersonId) as Count
		FROM Ranges R
		CROSS JOIN v_PersonHistory P 
		INNER JOIN dbo.MSBadge M ON M.PersonId = P.PersonId
		INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		WHERE   ISNULL(M.ExcludeInReports,0) = 0 AND
				P.HireDate <= R.EndDate AND 
				(P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate) AND 
				(P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)) AND
				(@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		GROUP BY R.StartDate,R.EndDate
	 ),
	 OrganicBreakPeople
	 AS
	 (
	   SELECT DISTINCT M.PersonId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.OrganicBreakEndDate AND M.OrganicBreakStartDate <= R.EndDate
	   INNER JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	 BadgedOnProject
	 AS
	 (
		SELECT DISTINCT R.PersonId,R.StartDate,R.EndDate,MAX(ISNULL(R.IsBadgeException,0)) AS Exception
		FROM
		(SELECT DISTINCT MP.PersonId,R.StartDate,R.EndDate,mpe.IsBadgeException 
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		INNER JOIN Ranges R ON MPE.BadgeStartDate <= R.EndDate AND R.StartDate <= MPE.BadgeEndDate
		INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
		INNER JOIN v_Pay pay ON pay.PersonId = MB.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		WHERE MB.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) AND mpe.IsbadgeRequired = 1 AND P.ProjectStatusId IN (1,2,3,4) 

		UNION ALL
		SELECT DISTINCT M.PersonId,R.StartDate,R.EndDate,0
		FROM dbo.MSBadge M
		INNER JOIN v_CurrentMSBadge MB ON MB.PersonId = M.PersonId
		INNER JOIN Ranges R ON M.LastBadgeStartDate <= R.EndDate AND R.StartDate <= M.LastBadgeEndDate
		INNER JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) 
			AND (M.LastBadgeStartDate <= MB.BadgeEndDate AND MB.BadgeStartDate <= M.LastBadgeEndDate)
		) R
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = R.StartDate AND OB.PersonId = R.PersonId
		WHERE OB.PersonId IS NULL
		GROUP BY R.PersonId,R.StartDate,R.EndDate
	 ),
	 BadgedNotOnProject
	 AS
	 (
		SELECT DISTINCT M.PersonId,R.StartDate,R.EndDate,MAX(CASE WHEN MB.IsException = 1 AND R.StartDate <= MB.ExceptionEndDate AND MB.ExceptionStartDate <= R.EndDate THEN 1 ELSE 0 END) AS Exception
		FROM v_CurrentMSBadge M 
		INNER JOIN Ranges R ON R.StartDate <= M.BadgeEndDate AND M.BadgeStartDate <= R.EndDate
		INNER JOIN dbo.MSBadge MB ON MB.PersonId = M.PersonId
		INNER JOIN v_Pay pay ON pay.PersonId = MB.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = R.StartDate AND BP.PersonId = M.PersonId
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = R.StartDate AND OB.PersonId = M.PersonId
		WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND OB.PersonId IS NULL AND
		(@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		AND (MB.IsBlocked = 0 OR (MB.IsBlocked = 1 AND (R.StartDate > MB.BlockEndDate OR MB.BlockStartDate > R.EndDate)))
		GROUP BY M.PersonId,R.StartDate,R.EndDate 
	 ),

	  BadgedNotOnProjectCount
	 AS
	 (
		SELECT R.StartDate,R.EndDate,SUM(CASE WHEN Exception = 0 THEN 1 ELSE 0 END) AS BadgedNotOnProjectCount, SUM(CASE WHEN Exception = 1 THEN 1 ELSE 0 END) AS BadgedNotOnProjectExceptionCount
		FROM BadgedNotOnProject R
		INNER JOIN v_PersonHistory P ON P.PersonId = R.PersonId AND P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
		WHERE P.personid IS NOT NULL AND (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
		GROUP BY R.StartDate,R.EndDate 
	 ),
	 BadgedProjectCount
	 AS
	 (
	    SELECT BP.StartDate,BP.EndDate,SUM(CASE WHEN Exception = 0 THEN 1 ELSE 0 END) AS BadgedOnProjectCount, SUM(CASE WHEN Exception = 1 THEN 1 ELSE 0 END) AS BadgedOnProjectExceptionCount
		FROM BadgedOnProject BP
		INNER JOIN v_PersonHistory P ON P.PersonId = BP.PersonId AND P.HireDate <= BP.EndDate AND (P.TerminationDate IS NULL OR BP.StartDate <= p.TerminationDate)
		WHERE (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
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
	   INNER JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	  InBreakPeriod
	 AS
	 (
	   SELECT M.PersonId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.BreakEndDate AND M.BreakStartDate <= R.EndDate
	   INNER JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR Pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   GROUP BY M.PersonId,R.StartDate,R.EndDate
	 ),
	 BlockedPeopleCount
	 AS
	 (
	   SELECT B.StartDate,B.EndDate,COUNT(B.PersonId) AS BlockedCount
	   FROM BlockedPeople B 
	   INNER JOIN v_PersonHistory P ON P.PersonId = B.PersonId AND P.HireDate <= B.EndDate AND (P.TerminationDate IS NULL OR B.StartDate <= p.TerminationDate)
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = B.StartDate AND BP.PersonId = B.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = B.StartDate AND BNP.PersonId = B.PersonId
	   LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = B.StartDate AND OB.PersonId = B.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND OB.PersonId IS NULL AND (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
	   GROUP BY B.StartDate,B.EndDate
	 ),
	  InBreakPeriodCount
	  AS
	  (
	   SELECT Brk.StartDate,Brk.EndDate,COUNT(Brk.PersonId) AS BlockedCount
	   FROM InBreakPeriod Brk 
	   INNER JOIN v_PersonHistory P ON P.PersonId = Brk.PersonId AND P.HireDate <= Brk.EndDate AND (P.TerminationDate IS NULL OR Brk.StartDate <= p.TerminationDate)
	   LEFT JOIN BlockedPeople B ON B.StartDate = Brk.StartDate AND B.PersonId = Brk.PersonId
	   LEFT JOIN BadgedOnProject BP ON BP.StartDate = Brk.StartDate AND BP.PersonId = Brk.PersonId
	   LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = Brk.StartDate AND BNP.PersonId = Brk.PersonId
	   LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = Brk.StartDate AND OB.PersonId = Brk.PersonId
	   WHERE BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND OB.PersonId IS NULL AND (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
	   GROUP BY Brk.StartDate,Brk.EndDate
	  ),
	  OrganicBreakCount
	  AS
	  (
	   SELECT Brk.StartDate,Brk.EndDate,COUNT(Brk.PersonId) AS BlockedCount
	   FROM OrganicBreakPeople Brk 
	   INNER JOIN v_PersonHistory P ON P.PersonId = Brk.PersonId AND P.HireDate <= Brk.EndDate AND (P.TerminationDate IS NULL OR Brk.StartDate <= p.TerminationDate)
	   WHERE (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
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

	 SELECT R.StartDate,R.EndDate,
			ISNULL(BP.BadgedOnProjectCount,0) AS BadgedOnProjectCount,
			ISNULL(BP.BadgedOnProjectExceptionCount,0) AS BadgedOnProjectExceptionCount,
			ISNULL(BNP.BadgedNotOnProjectCount,0) AS BadgedNotOnProjectCount,
			ISNULL(BNP.BadgedNotOnProjectExceptionCount,0) AS BadgedNotOnProjectExceptionCount,
			ISNULL(A.Count,0)-(ISNULL(BP.BadgedOnProjectCount,0) +ISNULL(BP.BadgedOnProjectExceptionCount,0)+ ISNULL(BNP.BadgedNotOnProjectCount,0)+ ISNULL(BNP.BadgedNotOnProjectExceptionCount,0)+ ISNULL(B.BlockedCount,0) + ISNULL(Brk.BlockedCount,0)+ ISNULL(OB.BlockedCount,0)) AS ClockNotStartedCount,
			ISNULL(B.BlockedCount,0) AS BlockedCount,
			ISNULL(Brk.BlockedCount,0)+ISNULL(OB.BlockedCount,0) AS InBreakPeriodCount
	 FROM Ranges R
	 LEFT JOIN ActiveConsultantsRange A ON A.StartDate = R.StartDate
	 LEFT JOIN BadgedProjectCount BP ON BP.StartDate = R.StartDate
	 LEFT JOIN BadgedNotOnProjectCount BNP ON BNP.StartDate = R.StartDate
	 --LEFT JOIN ClockNotStartedCount C ON C.StartDate = R.StartDate
	 LEFT JOIN BlockedPeopleCount B ON B.StartDate = R.StartDate
	 LEFT JOIN InBreakPeriodCount Brk ON Brk.StartDate = R.StartDate
	 LEFT JOIN OrganicBreakCount OB ON OB.StartDate = R.StartDate
	 ORDER BY R.StartDate
END

