CREATE PROCEDURE [dbo].[CheckIfValidDivision]
	(
		@PersonId	INT,
		@StartDate	DATETIME,
		@EndDate	DATETIME
	)
AS
BEGIN

	SELECT	TOP 1 *
	FROM	v_DivisionHistory DH 
	WHERE	DH.PersonId = @PersonId AND DH.StartDate <= @EndDate AND (DH.EndDate IS NULL OR @StartDate < DH.EndDate) AND ISNULL(DH.DivisionId,0) NOT IN (SELECT DivisionId FROM dbo.PersonDivision WHERE DivisionName IN ('Consulting','Business Development','Technology Consulting','Business Consulting','Director'))
	
END

