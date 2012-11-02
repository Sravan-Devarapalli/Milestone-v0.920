CREATE FUNCTION [dbo].[GetSickLeaveTimeTypeId]
(
)
RETURNS INT
AS
BEGIN

	RETURN (SELECT tt.TimeTypeId 
			FROM TimeType tt 
			WHERE tt.Code = 'W9311' /* Here 'W9311' is code of Sick/Safe Leave Work Type.*/
			)
END
