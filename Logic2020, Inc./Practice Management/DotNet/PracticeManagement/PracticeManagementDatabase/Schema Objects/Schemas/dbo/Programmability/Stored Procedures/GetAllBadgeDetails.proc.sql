CREATE PROCEDURE [dbo].[GetAllBadgeDetails]
(
	@PayTypeIds	NVARCHAR(MAX)=NULL,
	@PersonStatusIds NVARCHAR(MAX)
)
AS
BEGIN

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),@Today DATETIME,
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

	SELECT	P.PersonId,
			ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
			P.LastName,
			M.BadgeStartDate,
			M.BadgeEndDate,
			M.BreakStartDate,
			M.BreakEndDate,
			CP.Timescale,
			T.Name AS TimescaleName,
			DATEDIFF(MM,@Today,M.BadgeEndDate+1) BadgeDuration,
			P.TitleId,
			P.Title,
			MB.OrganicBreakStartDate,
			MB.OrganicBreakEndDate,
			DATEDIFF(MM,@Today,MB.OrganicBreakEndDate+1) OrganicBreakDuration
	FROM dbo.v_Person P
	INNER JOIN dbo.MSBadge MB ON MB.PersonId = P.PersonId
	LEFT JOIN v_CurrentMSBadge M ON M.PersonId = P.PersonId
	INNER JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
	LEFT JOIN dbo.Timescale T ON T.TimescaleId = CP.Timescale
	WHERE ISNULL(MB.ExcludeInReports,0) = 0  AND P.PersonStatusId IN (1,5) -- Active and Termination Pending
	AND P.IsStrawman = 0
	AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	AND P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
	ORDER BY P.LastName,ISNULL(P.PreferredFirstName,P.FirstName)

END

