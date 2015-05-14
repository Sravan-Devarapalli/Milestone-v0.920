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
			P.FirstName,
			P.LastName,
			M.BadgeStartDate,
			M.BadgeEndDate,
			M.BreakStartDate,
			M.BreakEndDate,
			CP.Timescale,
			T.Name AS TimescaleName,
			DATEDIFF(MM,@Today,M.BadgeEndDate+1) BadgeDuration,
			P.TitleId,
			P.Title
	FROM dbo.v_Person P
	LEFT JOIN dbo.MSBadge M ON M.PersonId = P.PersonId
	INNER JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
	LEFT JOIN dbo.Timescale T ON T.TimescaleId = CP.Timescale
	WHERE M.ExcludeInReports = 0 AND P.PersonStatusId IN (1,5) -- Active and Termination Pending
	AND P.IsStrawman = 0
	AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	AND P.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable)
	ORDER BY P.LastName,P.FirstName

END
