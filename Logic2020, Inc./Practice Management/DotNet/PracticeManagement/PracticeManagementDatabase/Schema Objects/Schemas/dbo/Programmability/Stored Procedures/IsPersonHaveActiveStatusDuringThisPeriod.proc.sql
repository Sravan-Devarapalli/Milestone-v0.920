CREATE PROCEDURE [dbo].[IsPersonHaveActiveStatusDuringThisPeriod]
(
	@PersonId           INT,
	@StartDate          DATETIME,
	@EndDate            DATETIME = NULL
)	
AS
BEGIN

	DECLARE @IsPersonHasActiveStatus BIT
	SET @IsPersonHasActiveStatus = 0
	
	DECLARE @MinStartdates TABLE (PersonId INT ,MinStartDate DATETIME,StatusId INT)
		
	INSERT INTO @MinStartdates (PersonId ,MinStartDate)
	SELECT @PersonId,MIN(PSH.StartDate) 
	FROM dbo.PersonStatusHistory PSH
	WHERE PSH.PersonId = @PersonId
		
	UPDATE minSD
	SET StatusId = PSH.PersonStatusId
	FROM @MinStartdates minSD
	INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = minSD.personId AND minSD.MinStartDate = PSH.StartDate


	SELECT @IsPersonHasActiveStatus = 1
	FROM dbo.Person AS p
	INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId 
	INNER JOIN @MinStartdates minSD ON minSD.personId = p.PersonId
	WHERE 
	p.IsStrawman = 0
	AND PSH.PersonStatusId = 1 -- ACTIVE Status
	AND (P.HireDate - (DATEPART(dw,P.HireDate) -1 )) <= @StartDate 
	AND  ( @StartDate < ISNULL(PSH.EndDate,dbo.GetFutureDate()) AND @EndDate > PSH.StartDate ) OR
		    ( minSD.StatusId = 1 AND  P.HireDate < minSD.MinStartDate AND @StartDate < minSD.MinStartDate AND @EndDate > P.HireDate )

    SELECT @IsPersonHasActiveStatus 
END
