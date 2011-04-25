-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-29
-- Description:	Gets default hours per day for given date
-- =============================================
CREATE FUNCTION GetDefaultHoursPerDay
(
	@PersonId INT,
	@Date DATETIME
)
RETURNS INT
AS
BEGIN
	DECLARE @res INT

	SELECT @res = SUM(DefaultHoursPerDay) 
	FROM dbo.v_Pay
	WHERE @Date BETWEEN StartDate AND ISNULL(EndDate, dbo.GetFutureDate()) AND 
		  @PersonId = PersonId

	RETURN ISNULL(@res, 8)
END

