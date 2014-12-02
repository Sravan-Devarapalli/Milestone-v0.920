CREATE FUNCTION [dbo].[GetPayTypeAtSpecificDate]
(
	@PersonId		INT, 
	@Date			DATETIME
)
RETURNS DATETIME 
AS
BEGIN
	DECLARE @result DATETIME -- VARCHAR(50)
	SET @result = NULL  --'n/a'
	
	;WITH CurrentPay AS (
	SELECT p.StartDate
	  FROM dbo.v_Pay AS p
	 WHERE p.PersonId = @PersonId
	   AND @Date >= p.StartDate
	   AND @Date < p.EndDateOrig
	UNION ALL
	SELECT TOP 1
	       p.StartDate
	  FROM dbo.v_Pay AS p
	 WHERE p.PersonId = @PersonId
	   AND NOT EXISTS(SELECT 1
	                    FROM dbo.v_Pay AS p
	                   WHERE p.PersonId = @PersonId
	                     AND @Date >= p.StartDate
	                     AND @Date < p.EndDateOrig)
	   AND @Date < p.StartDate
	)
	SELECT @result = c.StartDate
	FROM CurrentPay AS c
--	INNER JOIN dbo.Timescale AS t ON c.Timescale = t.TimescaleId
	
	RETURN @result
END

