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
	SELECT @IsPersonHasActiveStatus = 0,
			@FutureDate = dbo.GetFutureDate()

	SELECT @IsPersonHasActiveStatus = 1
	FROM dbo.Person AS p
	INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId
	WHERE p.IsStrawman = 0
	AND PSH.PersonStatusId = 1 -- ACTIVE Status
	AND (P.HireDate - (DATEPART(dw,P.HireDate) -1 )) <= @StartDate 
	AND  ( @StartDate < ISNULL(PSH.EndDate, @FutureDate) AND @EndDate > PSH.StartDate )

    SELECT @IsPersonHasActiveStatus 
END
