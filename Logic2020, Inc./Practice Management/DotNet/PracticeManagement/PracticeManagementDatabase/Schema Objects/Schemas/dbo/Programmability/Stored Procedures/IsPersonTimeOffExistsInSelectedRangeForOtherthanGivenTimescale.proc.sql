CREATE PROCEDURE [dbo].[IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale]
(
	@PersonId		INT ,
	@StartDate      DATETIME = NULL,
	@EndDate        DATETIME = NULL,
	@TimescaleId		INT
)	
AS
BEGIN
	
	DECLARE @IsTimeOffExists BIT = 0,		
			@IsW2SalaryTimeScale BIT,
			@IsW2HourlyTimeScale BIT

	SELECT	@IsW2SalaryTimeScale = CASE WHEN @TimescaleId = 2 THEN 1 ELSE 0 END,
			@IsW2HourlyTimeScale = CASE WHEN @TimescaleId = 1 THEN 1 ELSE 0 END

	IF EXISTS (	SELECT 1 
				FROM dbo.PersonCalendar PC
				INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = PC.TimeTypeId
				WHERE PC.PersonId = @PersonId 
						AND (@StartDate IS NULL OR @EndDate IS NULL OR (@StartDate IS NOT NULL AND @EndDate IS NOT NULL AND PC.DATE BETWEEN @StartDate AND @EndDate)) 
						AND (
								(@IsW2SalaryTimeScale = 0 AND TT.IsW2SalaryAllowed = 1 AND TT.IsW2HourlyAllowed != 1)
								OR 
								(@IsW2HourlyTimeScale = 0 AND TT.IsW2SalaryAllowed != 1 AND TT.IsW2HourlyAllowed = 1)
							)
				)
	BEGIN
		SET @IsTimeOffExists = 1
	END

	SELECT @IsTimeOffExists AS IsTimeOffExists

END
