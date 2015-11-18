CREATE PROCEDURE [dbo].[ManagedServiceReportByPerson]
(
	@PayTypeIds	NVARCHAR(MAX)=NULL,
	@PersonStatusIds NVARCHAR(MAX),
	@StartDate	DATETIME,
	@EndDate	DATETIME
) WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	SET ANSI_WARNINGS OFF

	DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@Today DATETIME

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),
			@PersonStatusIdsLocal NVARCHAR(MAX)

	SET @PayTypeIdsLocal = @PayTypeIds
	SET @PersonStatusIdsLocal = @PersonStatusIds
	SET @Today = dbo.GettingPMTime(GETUTCDATE())

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

	INSERT INTO @PersonStatusIdsTable(Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsLocal)

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)

	 SELECT DISTINCT P.PersonId,
		    P.First AS FirstName,
			P.LastName,
			CASE WHEN TS.TimescaleId = 3 THEN '1099-Hourly' ELSE TS.Name END AS TimeScaleName,
			P.TitleId,
			P.Title,
			CMS.BadgeStartDate,
			CMS.BadgeEndDate,
			CMS.BreakStartDate,
			CMS.BreakEndDate,
			LB.BlockStartDate,
			LB.BlockEndDate,
			LB.BlockEndDate+1 AS RestartDate,
			DATEDIFF(MM,@Today,CMS.BadgeEndDate+1) BadgeDuration,
			MS.ManageServiceContract
	 FROM v_Person P 
	 INNER JOIN v_PersonHistory PH ON PH.PersonId = P.PersonId
	 INNER JOIN [dbo].[GetCurrentPayTypeLatestTable]() LP ON LP.PersonId = P.PersonId
	 LEFT JOIN v_CurrentMSBadge CMS ON CMS.PersonId = P.PersonId
	 LEFT JOIN MSBadge MS ON MS.PersonId = P.PersonId 
	 LEFT JOIN [dbo].[GetLatestBlockDatesInTheGivenRange](@StartDateLocal,@EndDateLocal) LB ON LB.PersonId = P.PersonId
	 LEFT JOIN dbo.Timescale TS ON TS.TimescaleId = LP.Timescale
	 WHERE PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		  AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
		  AND (@PayTypeIds IS NULL OR LP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
		  AND MS.ExcludeInReports=0
		  AND P.ShowInMeetingReport=1

	  ;WITH Ranges
	  AS
      (
		SELECT  c.MonthStartDate as StartDate,c.MonthEndDate  AS EndDate
		FROM dbo.Calendar c
		WHERE c.date BETWEEN @StartDateLocal and @EndDateLocal
		GROUP BY c.MonthStartDate,c.MonthEndDate
	  ),
	 FilteredPeople
	 AS
	 (
		SELECT DISTINCT PH.PersonId, R.StartDate, R.EndDate
		FROM v_PersonHistory PH
		INNER JOIN [dbo].[GetCurrentPayTypeLatestTable]() pay ON pay.PersonId = PH.PersonId 
		CROSS JOIN Ranges R
		LEFT JOIN MSBadge MS ON MS.PersonId = PH.PersonId 
		LEFT JOIN v_Person P ON P.PersonId = PH.PersonId
		WHERE	MS.ExcludeInReports=0
				AND P.ShowInMeetingReport=1
				AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
				AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)) 
				AND (@PayTypeIds IS NULL OR pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
				
	 )
	SELECT	FP.PersonId,
			FP.StartDate,
			FP.EndDate,
			CASE WHEN PH.PersonId IS NULL OR M.PersonId IS NOT NULL OR MS.PersonId IS NOT NULL THEN 0
					ELSE 1 END AS IsAvailable			
	FROM FilteredPeople FP
	LEFT JOIN dbo.MSBadge M ON M.PersonId = FP.PersonId AND ((M.BlockStartDate <= FP.EndDate AND FP.StartDate <= M.BlockEndDate) OR (M.OrganicBreakStartDate <= FP.EndDate AND FP.StartDate <= M.OrganicBreakEndDate))
	LEFT JOIN v_CurrentMSBadge MS ON MS.PersonId = FP.PersonId AND (MS.BreakStartDate <= FP.EndDate AND FP.StartDate <= MS.BreakEndDate)
	LEFT JOIN v_PersonHistory PH ON PH.PersonId = FP.PersonId AND (PH.HireDate <= FP.StartDate AND (PH.TerminationDate IS NULL OR FP.EndDate <= PH.TerminationDate))
	ORDER BY FP.PersonId,
			 FP.StartDate,
			 FP.EndDate
END

