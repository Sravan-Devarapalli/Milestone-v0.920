-- ====================================================================================================
-- Author:		Sainathc
-- Create date: 05-10-2012
-- Description : Returns IsPerson SalaryType or not and hourly type or not for the given period of time.
-- ====================================================================================================
CREATE PROCEDURE [dbo].[IsPersonSalaryTypeListByPeriod]
(
	@PersonId   INT,
	@StartDate DATETIME ,
    @EndDate DATETIME
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT C.Date,
		   CASE WHEN ISNULL(P.Timescale,0) = 2 --2:W2-salary
					THEN CONVERT(BIT,1) 
				ELSE CONVERT(BIT,0) 
			END  AS IsSalaryType,--if there is no pay for any date consider as not salary Type
			 CASE WHEN ISNULL(P.Timescale,0) = 1 --1:W2-hourly
					THEN CONVERT(BIT,1) 
				ELSE CONVERT(BIT,0) 
			END  AS IsHourlyType--if there is no pay for any date consider as not hourly Type
	FROM dbo.Calendar C 
	LEFT JOIN dbo.Pay AS P ON C.date BETWEEN P.StartDate AND P.EndDate-1
		  					AND P.Person = @PersonId 
	WHERE C.date BETWEEN @StartDate AND @EndDate 
	ORDER BY C.Date 

END

