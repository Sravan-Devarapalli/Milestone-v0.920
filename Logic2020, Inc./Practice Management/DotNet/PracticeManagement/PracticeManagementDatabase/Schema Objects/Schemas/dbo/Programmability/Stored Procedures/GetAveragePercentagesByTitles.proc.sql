CREATE PROCEDURE [dbo].[GetAveragePercentagesByTitles]
(
	@PayTypeIds			NVARCHAR(MAX)=NULL,
	@PersonStatusIds	NVARCHAR(MAX),
	@TitleIds			NVARCHAR(MAX),
	@StartDate			DATETIME,
	@EndDate			DATETIME
)WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	SET ANSI_WARNINGS OFF
	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal DATETIME

	DECLARE @TitleIdsTable TABLE ( Ids INT )
	
	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )

	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),
		@PersonStatusIdsLocal NVARCHAR(MAX)
	SET @PayTypeIdsLocal = @PayTypeIds
	SET @PersonStatusIdsLocal = @PersonStatusIds

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

	INSERT INTO @TitleIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@TitleIds)

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
		GROUP BY c.MonthStartDate,c.MonthEndDate  
	)
	,
     ActiveConsultantsRange
	 AS
	 (
		SELECT per.TitleId,R.StartDate,R.EndDate,COUNT(DISTINCT per.PersonId) as Count
		FROM Ranges R
		CROSS JOIN Person per 
		LEFT JOIN dbo.MSBadge M ON M.PersonId = per.PersonId
		LEFT JOIN v_PersonHistory P ON per.PersonId = p.PersonId
		LEFT JOIN v_Pay pay ON pay.PersonId = P.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		WHERE   (per.TitleId IN (SELECT Ids FROM @TitleIdsTable)) AND
				ISNULL(M.ExcludeInReports,0) = 0 AND
				P.HireDate <= R.EndDate AND 
				(P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate) AND 
				(P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)) AND
				(@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) 
		GROUP BY per.TitleId,R.StartDate,R.EndDate
	 ),
	 OrganicBreakPeople
	 AS
	 (
	   SELECT DISTINCT M.PersonId,P.TitleId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN Ranges R ON R.StartDate <= M.OrganicBreakEndDate AND M.OrganicBreakStartDate <= R.EndDate
	   LEFT JOIN dbo.Person P ON P.PersonId = M.PersonId
	   LEFT JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   AND P.TitleId IN (SELECT Ids FROM @TitleIdsTable)
	   GROUP BY M.PersonId,P.TitleId,R.StartDate,R.EndDate
	 ),
	 BadgedOnProject
	 AS
	 (
		SELECT DISTINCT P.PersonId,P.TitleId,P.StartDate,P.EndDate
		FROM
	    (	
			SELECT DISTINCT MP.PersonId,P.TitleId,R.StartDate,R.EndDate
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project Pr ON Pr.ProjectId = M.ProjectId
			INNER JOIN Ranges R ON MPE.BadgeStartDate <= R.EndDate AND R.StartDate <= MPE.BadgeEndDate
			INNER JOIN dbo.Person P ON P.PersonId = MP.PersonId 
			LEFT JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
			LEFT JOIN v_Pay pay ON pay.PersonId = P.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
			WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)) AND Pr.ProjectStatusId IN (1,2,3,4) 

			UNION ALL
			SELECT DISTINCT M.PersonId,P.TitleId,R.StartDate,R.EndDate
			FROM dbo.MSBadge M
			INNER JOIN v_CurrentMSBadge MB ON M.PersonId = MB.PersonId
			INNER JOIN Person P ON P.PersonId = M.PersonId 
			INNER JOIN Ranges R ON M.LastBadgeStartDate <= R.EndDate AND R.StartDate <= M.LastBadgeEndDate
			LEFT JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
			WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
					AND (M.LastBadgeStartDate <= MB.BadgeEndDate AND MB.BadgeStartDate <= M.LastBadgeEndDate)
		)P
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = P.StartDate AND OB.PersonId = P.PersonId
		WHERE OB.PersonId IS NULL AND P.TitleId IN (SELECT Ids FROM @TitleIdsTable)
		GROUP BY P.PersonId,P.TitleId,P.StartDate,P.EndDate
	 ),
	 BadgedNotOnProject
	 AS
	 (
		SELECT DISTINCT M.PersonId,P.TitleId,R.StartDate,R.EndDate
		FROM v_CurrentMSBadge M 
		INNER JOIN Ranges R ON R.StartDate <= M.BadgeEndDate AND M.BadgeStartDate <= R.EndDate
		LEFT JOIN dbo.Person P ON P.PersonId = M.PersonId
		LEFT JOIN dbo.MSBadge MB ON MB.PersonId = P.PersonId
		LEFT JOIN v_Pay pay ON pay.PersonId = MB.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = R.StartDate AND BP.PersonId = M.PersonId
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = R.StartDate AND OB.PersonId = M.PersonId
		WHERE OB.PersonId IS NULL AND M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
			  AND P.TitleId IN (SELECT Ids FROM @TitleIdsTable)
			  AND (MB.IsBlocked = 0 OR (MB.IsBlocked = 1 AND (R.StartDate > MB.BlockEndDate OR MB.BlockStartDate > R.EndDate)))
		GROUP BY M.PersonId,P.TitleId,R.StartDate,R.EndDate 
	 ),
	  BadgedNotOnProjectCount
	 AS
	 (
		SELECT R.TitleId,R.StartDate,R.EndDate,COUNT(DISTINCT R.PersonId) AS BadgedNotOnProjectCount 
		FROM BadgedNotOnProject R
		LEFT JOIN v_PersonHistory P ON P.PersonId = R.PersonId AND P.HireDate <= R.EndDate AND (P.TerminationDate IS NULL OR R.StartDate <= p.TerminationDate)
		WHERE P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
		GROUP BY R.TitleId,R.StartDate,R.EndDate 
	 ),
	 BadgedProjectCount
	 AS
	 (
	    SELECT BP.TitleId,BP.StartDate,BP.EndDate,COUNT(DISTINCT BP.PersonId) AS BadgedOnProjectCount
		FROM BadgedOnProject BP
		LEFT JOIN v_PersonHistory P ON P.PersonId = BP.PersonId AND P.HireDate <= BP.EndDate AND (P.TerminationDate IS NULL OR BP.StartDate <= p.TerminationDate)
		WHERE P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
		GROUP BY BP.TitleId,BP.StartDate,BP.EndDate
	 ),
	  BlockedPeople
	 AS
	 (
	    SELECT DISTINCT M.PersonId,P.TitleId,R.StartDate,R.EndDate
	   FROM dbo.MSBadge M 
	   JOIN dbo.Person P ON P.PersonId = M.PersonId
	   JOIN Ranges R ON R.StartDate <= M.BlockEndDate AND M.BlockStartDate <= R.EndDate
	   LEFT JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	   AND P.TitleId IN (SELECT Ids FROM @TitleIdsTable)
	   GROUP BY M.PersonId,P.TitleId,R.StartDate,R.EndDate
	 ),
	  InBreakPeriod
	 AS
	 (
	   SELECT DISTINCT M.PersonId,P.TitleId,R.StartDate,R.EndDate
	   FROM v_CurrentMSBadge M 
	   JOIN dbo.Person P ON P.PersonId = M.PersonId
	   JOIN Ranges R ON R.StartDate <= M.BreakEndDate AND M.BreakStartDate <= R.EndDate
	   LEFT JOIN v_Pay pay ON pay.PersonId = M.PersonId AND pay.StartDate <= R.EndDate AND R.StartDate < pay.EndDateOrig
	   WHERE M.ExcludeInReports = 0 AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	    AND P.TitleId IN (SELECT Ids FROM @TitleIdsTable)
	   GROUP BY M.PersonId,P.TitleId,R.StartDate,R.EndDate
	 ),
	  BlockedPeopleCount
	 AS
	 (
	    SELECT B.TitleId,B.StartDate,B.EndDate,COUNT(DISTINCT B.PersonId) AS BlockedPeopleCount
		FROM BlockedPeople B
		LEFT JOIN v_PersonHistory P ON P.PersonId = B.PersonId AND P.HireDate <= B.EndDate AND (P.TerminationDate IS NULL OR B.StartDate <= p.TerminationDate)
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = B.StartDate AND BP.PersonId = B.PersonId
	    LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = B.StartDate AND BNP.PersonId = B.PersonId
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = B.StartDate AND OB.PersonId = B.PersonId
	    WHERE OB.PersonId IS NULL AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND 
			  (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
	    GROUP BY B.TitleId,B.StartDate,B.EndDate
	 ),
	  InBreakPeriodCount
	  AS
	  (
		SELECT B.TitleId,B.StartDate,B.EndDate,COUNT(DISTINCT B.PersonId) AS BreakCount
		FROM InBreakPeriod B
		LEFT JOIN v_PersonHistory P ON P.PersonId = B.PersonId AND P.HireDate <= B.EndDate AND (P.TerminationDate IS NULL OR B.StartDate <= p.TerminationDate)
		LEFT JOIN BlockedPeople Blck ON Blck.StartDate = B.StartDate AND Blck.PersonId = B.PersonId
		LEFT JOIN BadgedOnProject BP ON BP.StartDate = B.StartDate AND BP.PersonId = B.PersonId
	    LEFT JOIN BadgedNotOnProject BNP ON BNP.StartDate = B.StartDate AND BNP.PersonId = B.PersonId
		LEFT JOIN OrganicBreakPeople OB ON OB.StartDate = B.StartDate AND OB.PersonId = B.PersonId
	    WHERE OB.PersonId IS NULL AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
				and P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
	    GROUP BY B.TitleId,B.StartDate,B.EndDate
	  ),
	  OrganicBreakCount
	  AS
	  (
		   SELECT Brk.TitleId,Brk.StartDate,Brk.EndDate,COUNT(Brk.PersonId) AS BlockedCount
		   FROM OrganicBreakPeople Brk 
		   LEFT JOIN v_PersonHistory P ON P.PersonId = Brk.PersonId AND P.HireDate <= Brk.EndDate AND (P.TerminationDate IS NULL OR Brk.StartDate <= p.TerminationDate)
		   WHERE (P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
		   GROUP BY  Brk.TitleId,Brk.StartDate,Brk.EndDate
	  )
		 SELECT T.TitleId,
				T.Title,
				R.StartDate,
				R.EndDate,
				ISNULL(A.Count,0)-(ISNULL(B.BlockedPeopleCount,0) + ISNULL(Brk.BreakCount,0)+ ISNULL(OB.BlockedCount,0)) AS TotalCount
		 FROM Ranges R
		 CROSS JOIN dbo.Title T 
		 LEFT JOIN ActiveConsultantsRange A ON A.StartDate = R.StartDate AND A.TitleId = T.TitleId
		 LEFT JOIN BlockedPeopleCount B ON B.StartDate = R.StartDate AND B.TitleId = T.TitleId
		 LEFT JOIN InBreakPeriodCount Brk ON Brk.StartDate = R.StartDate AND Brk.TitleId = T.TitleId
		 LEFT JOIN OrganicBreakCount OB ON OB.StartDate = R.StartDate AND OB.TitleId = T.TitleId
		 WHERE T.TitleId IN (SELECT Ids FROM @TitleIdsTable)

	     OPTION (RECOMPILE);

END
