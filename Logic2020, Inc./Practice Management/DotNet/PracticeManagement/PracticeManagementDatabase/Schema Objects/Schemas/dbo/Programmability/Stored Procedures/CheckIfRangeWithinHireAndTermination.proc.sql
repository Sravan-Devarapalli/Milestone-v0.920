CREATE PROCEDURE [dbo].[CheckIfRangeWithinHireAndTermination]
	(
		@PersonId	INT,
		@StartDate	DATETIME,
		@EndDate	DATETIME
	)
AS
BEGIN

	DECLARE @ValidRange BIT

	IF EXISTS(
				SELECT	1
				FROM	v_PersonHistory PH 
				WHERE	PH.PersonId = @PersonId AND (PH.HireDate <= @StartDate AND (PH.TerminationDate IS NULL OR PH.TerminationDate >= @EndDate OR PH.PersonStatusId <> 2))
			)
		SET @ValidRange  = 1
	ELSE
		SET @ValidRange = 0

	SELECT @ValidRange

END
