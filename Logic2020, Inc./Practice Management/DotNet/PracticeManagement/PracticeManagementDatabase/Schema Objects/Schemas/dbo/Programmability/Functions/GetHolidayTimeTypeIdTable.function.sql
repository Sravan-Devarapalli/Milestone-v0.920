CREATE FUNCTION [dbo].[GetHolidayTimeTypeIdTable]
(
)
RETURNS @Result TABLE 
(
	HolidayTimeTypeId INT
)
AS
BEGIN
	
	INSERT INTO @Result
	SELECT tt.TimeTypeId 
	FROM TimeType tt 
	WHERE tt.Code = 'W9320' --Here 'W9320' is code of Holiday Work Type.

	RETURN
END
