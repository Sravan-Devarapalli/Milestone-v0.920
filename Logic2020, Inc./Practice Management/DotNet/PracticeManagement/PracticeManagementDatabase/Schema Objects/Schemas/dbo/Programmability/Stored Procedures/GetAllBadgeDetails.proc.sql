CREATE PROCEDURE [dbo].[GetAllBadgeDetails]
(
	@PayTypeIds	NVARCHAR(MAX)=NULL
)
AS
BEGIN

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),@Today DATETIME
	SET @PayTypeIdsLocal = @PayTypeIds

	SET @Today = dbo.GettingPMTime(GETUTCDATE())

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

	SELECT	P.PersonId,
			P.FirstName,
			P.LastName,
			M.BadgeStartDate,
			M.BadgeEndDate,
			M.BreakStartDate,
			M.BreakEndDate,
			CP.Timescale,
			T.Name AS TimescaleName,
			DATEDIFF(MM,@Today,M.BadgeEndDate+1) BadgeDuration
	FROM dbo.Person P
	LEFT JOIN dbo.MSBadge M ON M.PersonId = P.PersonId
	INNER JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
	LEFT JOIN dbo.Timescale T ON T.TimescaleId = CP.Timescale
	WHERE P.PersonStatusId IN (1,5) -- Active and Termination Pending
	AND P.IsStrawman = 0
	AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
	ORDER BY P.LastName,P.FirstName

END
