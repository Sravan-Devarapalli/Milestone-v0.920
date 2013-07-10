--Returns latest pay details in the given date range for the given person.
--If the person doesn't have the any pay in the given range returns NO RECORDS.
CREATE FUNCTION [dbo].[GetLatestPayInTheGivenRangeTable]
(
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
RETURNS TABLE
AS
	RETURN 
	SELECT TOP 1 P.Person AS PersonId,P.TitleId,T.Title,P.StartDate,CASE WHEN P.EndDate IS NOT NULL THEN P.EndDate-1 ELSE NULL END EndDate
	FROM dbo.Pay P 
	INNER JOIN dbo.Title T ON T.TitleId = P.TitleId 
	WHERE P.Person = @PersonId AND @StartDate <= P.EndDate-1 AND P.StartDate <= @EndDate
	ORDER BY P.StartDate DESC

