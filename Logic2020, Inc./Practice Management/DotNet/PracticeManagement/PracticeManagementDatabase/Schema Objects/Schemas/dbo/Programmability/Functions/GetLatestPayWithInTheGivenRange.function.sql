CREATE FUNCTION [dbo].[GetLatestPayWithInTheGivenRange]
(
	@Startdate DATETIME, 
	@Enddate DATETIME
)
RETURNS TABLE
/*
Gets the person's Latest pay type for the given range
*/
AS
RETURN
(
	WITH PersonPaysWithRank
	AS 
	(
	SELECT p.Person,p.StartDate,p.Timescale,RANK() OVER (PARTITION BY P.Person ORDER BY p.startdate DESC) LatestPayRank
	FROM dbo.Pay AS p
	INNER JOIN dbo.Person per on per.PersonId = p.Person AND per.IsStrawman = 0
	WHERE   @Startdate <= (p.EndDate-1) AND p.StartDate <= @Enddate
	)
	SELECT P.Person AS PersonId,P.Timescale
	FROM PersonPaysWithRank P
	WHERE p.LatestPayRank = 1
 )
