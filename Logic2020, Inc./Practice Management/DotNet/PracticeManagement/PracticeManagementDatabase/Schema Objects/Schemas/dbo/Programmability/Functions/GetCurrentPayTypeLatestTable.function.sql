CREATE FUNCTION [dbo].[GetCurrentPayTypeLatestTable]()
RETURNS TABLE
/*
Gets the person's current pay type i.e. Today's payType
a.If the person does no have today pay Then gets the first future's pay type 
b.if the person doesn't have the future's pay Then get last pay type in the past
*/
AS
RETURN
(
	WITH PersonPayToday
	AS 
	(
		SELECT p.Person,p.StartDate
		FROM dbo.Pay AS p
		WHERE  GETDATE() >= p.StartDate
				AND GETDATE() < p.EndDate
	),
	PersonPayAfterToday
	AS 
	(
		SELECT p.Person, MIN(p.StartDate) AS StartDate
		FROM dbo.Pay AS p
		LEFT JOIN PersonPayToday PPT ON p.Person = PPT.Person
		WHERE PPT.Person IS NULL
			AND GETDATE() < p.StartDate
		GROUP BY p.Person
	),
	PersonPayBeforeToday
	AS
	(
		SELECT p.Person, MAX(p.StartDate) AS StartDate
		FROM dbo.Pay AS p
		LEFT JOIN PersonPayToday PPT ON p.Person = PPT.Person
		LEFT JOIN PersonPayAfterToday PPAT ON PPAT.Person = p.Person
		WHERE PPT.Person IS NULL AND PPAT.Person IS NULL
			  AND GETDATE() > p.EndDate-1
		GROUP BY p.Person
	)

	SELECT P.Person AS PersonId ,P.Timescale
	FROM dbo.Pay P
	LEFT JOIN PersonPayToday PPT ON P.Person = PPT.Person AND P.StartDate = PPT.StartDate
	LEFT JOIN PersonPayAfterToday PPAT ON P.Person = PPAT.Person AND P.StartDate = PPAT.StartDate
	LEFT JOIN PersonPayBeforeToday PPBT ON P.Person = PPBT.Person AND P.StartDate = PPBT.StartDate
	WHERE PPT.Person IS NOT NULL OR PPAT.Person IS NOT NULL OR PPBT.Person IS NOT NULL

  )
