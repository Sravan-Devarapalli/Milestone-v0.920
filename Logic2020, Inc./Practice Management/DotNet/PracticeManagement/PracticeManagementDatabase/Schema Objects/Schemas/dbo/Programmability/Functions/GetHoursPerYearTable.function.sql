CREATE FUNCTION [dbo].[GetHoursPerYearTable]
(
)
RETURNS @Result table
(
	HoursPerYear DECIMAL
)
AS
BEGIN
	
	INSERT INTO @Result
	SELECT 2080

	RETURN
END
