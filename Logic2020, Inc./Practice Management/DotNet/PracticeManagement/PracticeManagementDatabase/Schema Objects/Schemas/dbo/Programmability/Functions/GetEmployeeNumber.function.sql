-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-02
-- Description:	Gets Employee number
-- =============================================
CREATE FUNCTION dbo.GetEmployeeNumber
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
	WHERE ((@StartDate >= pa.StartDate AND @StartDate <= pa.EndDate) OR
		(@EndDate IS NULL OR @EndDate <= pa.EndDate)) AND
		(pa.Timescale = 1 OR pa.Timescale = 2) AND
		(pe.PersonStatusId not in (3, 4))

	RETURN @Num
END

