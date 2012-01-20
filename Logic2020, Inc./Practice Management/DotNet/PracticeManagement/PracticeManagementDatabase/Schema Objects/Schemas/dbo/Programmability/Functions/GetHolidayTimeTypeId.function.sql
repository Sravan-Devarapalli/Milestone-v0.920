CREATE FUNCTION [dbo].[GetHolidayTimeTypeId] 
(
)
RETURNS INT
AS
BEGIN

	DECLARE @Id INT

	SELECT @Id = tt.TimeTypeId 
	FROM TimeType tt 
	WHERE tt.Code = 'W9320' --Here 'W9320' is code of Holiday Work Type.
	
	return @Id
END

