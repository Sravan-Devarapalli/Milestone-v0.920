CREATE PROCEDURE [dbo].[CheckIfPersonConsultantTypeInAPeriod]
	(
		@PersonId	INT,
		@StartDate	DATETIME,
		@EndDate	DATETIME
	)
AS
BEGIN

	DECLARE @NotValidPerson		BIT,
			@W2SalaryId			INT,
			@W2HourlyId			INT
	
	SELECT	@W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT	@W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
	
	IF EXISTS(
				SELECT	1
				FROM	v_Pay P 
				WHERE	P.PersonId = @PersonId AND P.StartDate <= @EndDate AND (@StartDate < P.EndDateOrig) AND P.Timescale NOT IN (@W2SalaryId,@W2HourlyId)
			)
		SET @NotValidPerson  = 1
	ELSE
		SET @NotValidPerson = 0

	SELECT @NotValidPerson

END
