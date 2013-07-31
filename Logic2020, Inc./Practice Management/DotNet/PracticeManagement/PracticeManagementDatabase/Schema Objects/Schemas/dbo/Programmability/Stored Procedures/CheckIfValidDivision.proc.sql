CREATE PROCEDURE [dbo].[CheckIfValidDivision]
	(
		@PersonId	INT,
		@StartDate	DATETIME,
		@EndDate	DATETIME
	)
AS
BEGIN

	DECLARE @NotValidPerson		BIT,
			@ConsultingDivId	INT,
			@BusinessDevelopmentDivId INT
	
	SELECT    @ConsultingDivId = DivisionId FROM dbo.PersonDivision WHERE DivisionName = 'Consulting'
	SELECT    @BusinessDevelopmentDivId = DivisionId FROM dbo.PersonDivision WHERE DivisionName = 'Business Development'
	
	IF EXISTS(
				SELECT	1
				FROM	v_DivisionHistory DH 
				WHERE	DH.PersonId = @PersonId AND DH.StartDate <= @EndDate AND (DH.EndDate IS NULL OR @StartDate < DH.EndDate) AND ISNULL(DH.DivisionId,0) NOT IN (@ConsultingDivId,@BusinessDevelopmentDivId)
			)
		SET @NotValidPerson  = 1
	ELSE
		SET @NotValidPerson = 0

	SELECT @NotValidPerson

END
