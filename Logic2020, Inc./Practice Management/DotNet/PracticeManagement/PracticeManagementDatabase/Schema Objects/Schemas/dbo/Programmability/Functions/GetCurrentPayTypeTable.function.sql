CREATE FUNCTION GetCurrentPayTypeTable()
RETURNS TABLE
/*
Gets the person's current pay type i.e. Today's payType
a.If the person does no have today pay Then gets the first future's pay type 
b.if the person doesn't have the future's pay Then
	Those persons are not returned by the result list.
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
	)

	SELECT P.Person AS PersonId ,P.Timescale
	FROM dbo.Pay P
	LEFT JOIN PersonPayToday PPT ON P.Person = PPT.Person AND P.StartDate = PPT.StartDate
	LEFT JOIN PersonPayAfterToday PPAT ON P.Person = PPAT.Person AND P.StartDate = PPAT.StartDate
	WHERE PPT.Person IS NOT NULL OR PPAT.Person IS NOT NULL

  )

