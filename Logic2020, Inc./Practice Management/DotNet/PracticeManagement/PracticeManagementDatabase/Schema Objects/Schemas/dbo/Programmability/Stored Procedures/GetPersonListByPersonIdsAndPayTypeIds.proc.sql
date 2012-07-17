CREATE PROCEDURE [dbo].[GetPersonListByPersonIdsAndPayTypeIds]
	@PersonIds	NVARCHAR(MAX),
	@TimescaleIds NVARCHAR(4000) = NULL,
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@PracticeIds  NVARCHAR(MAX)
AS
BEGIN

	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

	DECLARE @PersonList TABLE (Id int)
	INSERT INTO @PersonList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PersonIds)

	DECLARE @PracticeIdsList table (Id INT)
	INSERT INTO @PracticeIdsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)
	


	IF @TimescaleIds IS NOT NULL
	BEGIN

		DECLARE @TimescaleIdList table (Id int)
		INSERT INTO @TimescaleIdList
		SELECT * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)

	END


	SELECT DISTINCT P.PersonId,
			P.LastName,
			P.FirstName,
			P.IsDefaultManager
	FROM Person P
	JOIN @PersonList PL ON PL.Id = P.PersonId
	JOIN Pay pa ON pa.Person = P.PersonId AND pa.StartDate <= @EndDate AND (ISNULL(pa.EndDate, @FutureDate) - 1) >= @StartDate
	WHERE (@TimescaleIds IS NULL OR pa.Timescale IN (SELECT ID FROM @TimescaleIdList)) 
	       AND ((@PracticeIds IS NULL) OR ISNULL(pa.PracticeId,P.DefaultPractice) IN (SELECT Id FROM @PracticeIdsList))
END
