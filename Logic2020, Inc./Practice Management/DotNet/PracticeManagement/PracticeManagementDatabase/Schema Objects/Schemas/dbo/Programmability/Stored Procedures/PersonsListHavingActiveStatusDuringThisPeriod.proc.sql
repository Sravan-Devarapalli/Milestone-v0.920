CREATE PROCEDURE [dbo].[PersonsListHavingActiveStatusDuringThisPeriod]
(
	@StartDate     DATETIME ,
	@EndDate       DATETIME 
)
AS
BEGIN
		
		DECLARE @MinStartdates TABLE (personId INT ,minStartDate DATETIME,statusId INT)
		
		INSERT INTO @MinStartdates (personId ,minStartDate)
		SELECT PSH.PersonId,MIN(PSH.StartDate) 
		FROM dbo.PersonStatusHistory PSH
		GROUP BY PSH.PersonId 
		
		UPDATE minSD
		SET statusId = PSH.PersonStatusId
		FROM @MinStartdates minSD
		INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = minSD.personId AND minSD.minStartDate = PSH.StartDate


		SELECT p.PersonId,
		p.FirstName,
		p.LastName,
		p.IsDefaultManager
		FROM dbo.Person AS p
		INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId 
		INNER JOIN @MinStartdates minSD ON minSD.personId = p.PersonId
		WHERE 
		p.IsStrawman = 0
		AND PSH.PersonStatusId = 1 -- ACTIVE Status
		AND P.HireDate < @StartDate 
		AND  ( @StartDate < ISNULL(PSH.EndDate,dbo.GetFutureDate()) AND @EndDate > PSH.StartDate ) OR
		     ( minSD.statusId = 1 AND  P.HireDate < minSD.minStartDate AND @StartDate < minSD.minStartDate AND @EndDate > P.HireDate )
		ORDER BY p.LastName, p.FirstName

END
