CREATE FUNCTION [dbo].[GetHolidayTimeTypeId] 
(
)
RETURNS INT
AS
BEGIN

	RETURN (SELECT tt.TimeTypeId 
			FROM TimeType tt 
			WHERE tt.Code = 'W9320' /* Here 'W9320' is code of Holiday Work Type.*/
			)
END
