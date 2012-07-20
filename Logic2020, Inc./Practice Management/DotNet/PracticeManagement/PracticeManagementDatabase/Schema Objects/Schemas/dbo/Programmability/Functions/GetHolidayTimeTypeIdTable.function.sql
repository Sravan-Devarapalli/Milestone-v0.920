CREATE FUNCTION [dbo].[GetHolidayTimeTypeIdTable]
(
)
RETURNS TABLE 
AS
	RETURN SELECT tt.TimeTypeId AS HolidayTimeTypeId
			FROM TimeType tt 
			WHERE tt.Code = 'W9320' --Here 'W9320' is code of Holiday Work Type.
GO
