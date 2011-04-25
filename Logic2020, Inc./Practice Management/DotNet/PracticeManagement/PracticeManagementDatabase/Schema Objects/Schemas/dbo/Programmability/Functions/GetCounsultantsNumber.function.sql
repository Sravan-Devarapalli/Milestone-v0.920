-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-02
-- Description:	Gets Counsultant number
-- =============================================
CREATE FUNCTION dbo.GetCounsultantsNumber
(
	@StartDate           DATETIME,
	@EndDate             DATETIME
)
RETURNS int
AS
BEGIN
	DECLARE @Num int

	SELECT @Num = COUNT(DISTINCT pe.PersonId) FROM dbo.Person pe 
	INNER JOIN dbo.Pay AS pa ON pe.PersonId = pa.Person
	INNER JOIN dbo.Practice AS pr ON pe.DefaultPractice = pr.PracticeId 
	WHERE ((@StartDate >= pa.StartDate AND @StartDate <= pa.EndDate) OR
		(@EndDate IS NULL OR @EndDate <= pa.EndDate)) AND
		(pr.IsCompanyInternal = 0) AND
		(pe.PersonStatusId not in (3, 4))

	RETURN @Num
END

