-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-09
-- Description:	Returns person's current pay type
-- =============================================
CREATE FUNCTION GetCurrentPayType
(
	@PersonId int
)
RETURNS INT --VARCHAR(50) 
AS
BEGIN
	DECLARE @result INT -- VARCHAR(50)
	SET @result = NULL  --'n/a'

	DECLARE @Now DATETIME
	SET @Now = GETDATE();

	WITH CurrentPay AS (
	SELECT p.Timescale
	  FROM dbo.v_Pay AS p
	 WHERE p.PersonId = @PersonId
	   AND @Now >= p.StartDate
	   AND @Now < p.EndDateOrig
	UNION ALL
	SELECT TOP 1
	       p.Timescale
	  FROM dbo.v_Pay AS p
	 WHERE p.PersonId = @PersonId
	   AND NOT EXISTS(SELECT 1
	                    FROM dbo.v_Pay AS p
	                   WHERE p.PersonId = @PersonId
	                     AND @Now >= p.StartDate
	                     AND @Now < p.EndDateOrig)
	   AND @Now < p.StartDate
	)
	SELECT @result = c.Timescale
	FROM CurrentPay AS c
--	INNER JOIN dbo.Timescale AS t ON c.Timescale = t.TimescaleId
	
	RETURN @result
END

