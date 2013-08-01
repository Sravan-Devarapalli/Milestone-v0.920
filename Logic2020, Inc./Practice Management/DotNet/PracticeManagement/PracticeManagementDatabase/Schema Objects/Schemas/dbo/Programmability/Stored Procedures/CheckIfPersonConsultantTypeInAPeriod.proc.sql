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
				SELECT	*
				FROM	v_PayTimescaleHistory P 
				WHERE	P.PersonId = @PersonId AND P.StartDate <= @StartDate  AND (@EndDate <=  P.EndDate) AND P.Timescale IN (@W2SalaryId,@W2HourlyId)
			)
		SET @NotValidPerson  = 0
	ELSE
		SET @NotValidPerson = 1

	SELECT @NotValidPerson

END
