CREATE PROCEDURE [dbo].[IsPersonSalaryTypeInGivenRange]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
BEGIN
	DECLARE @IsSalaryType NVARCHAR(10)='False'	
	IF EXISTS(	SELECT 1
				FROM  Pay
				WHERE Person = @PersonId
					  AND StartDate <= @EndDate AND @StartDate <= EndDate-1 
					  AND Timescale = 2 -- W2 SALARY
			 )
	BEGIN
		   SET @IsSalaryType = 'True'
	END
	SELECT @IsSalaryType AS IsSalary
END

