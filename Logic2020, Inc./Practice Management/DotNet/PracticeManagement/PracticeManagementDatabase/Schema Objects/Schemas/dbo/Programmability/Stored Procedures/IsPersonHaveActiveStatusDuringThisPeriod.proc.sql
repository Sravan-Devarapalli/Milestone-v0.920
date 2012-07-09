CREATE PROCEDURE [dbo].[IsPersonHaveActiveStatusDuringThisPeriod]
(
	@PersonId           INT,
	@StartDate          DATETIME,
	@EndDate            DATETIME = NULL
)	
AS
BEGIN

	DECLARE @IsPersonHasActiveStatus BIT,
			@FutureDate	DATETIME

	SELECT  @IsPersonHasActiveStatus = 0,
			@FutureDate = dbo.GetFutureDate()

	SELECT @IsPersonHasActiveStatus = 1
	FROM dbo.Person AS p
	INNER JOIN dbo.PersonStatusHistory PSH ON  PSH.PersonId = p.PersonId AND p.IsStrawman = 0 AND P.PersonId = @PersonId AND PSH.PersonStatusId = 1 -- ACTIVE Status
	WHERE P.HireDate <= @EndDate AND ISNULL(P.TerminationDate,@FutureDate)  >= @StartDate 
		  AND PSH.StartDate <= @EndDate AND ISNULL(PSH.EndDate,@FutureDate)  >= @StartDate

    SELECT @IsPersonHasActiveStatus 
END
