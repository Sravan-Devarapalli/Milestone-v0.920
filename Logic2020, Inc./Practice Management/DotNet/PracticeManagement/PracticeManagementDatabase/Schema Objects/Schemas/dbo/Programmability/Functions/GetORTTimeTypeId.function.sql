CREATE FUNCTION [dbo].[GetORTTimeTypeId]()
RETURNS INT
AS
BEGIN
	DECLARE @Id	INT

	SELECT @Id = TimeTypeId
	FROM TimeType
	WHERE Code = 'W9300'--Here 'W9310' is the code of PTO work type.

	RETURN @Id
END
