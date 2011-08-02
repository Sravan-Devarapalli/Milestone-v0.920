CREATE FUNCTION [dbo].[GetHolidayTimeTypeId] 
(
)
RETURNS INT
AS
BEGIN

	DECLARE @Id INT

	SELECT @Id = tt.TimeTypeId 
	FROM TimeType tt 
	WHERE tt.Name = 'Holiday' --Here Holiday is the Time Type Name.
	
	return @Id
END

