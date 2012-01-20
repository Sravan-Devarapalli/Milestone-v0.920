CREATE FUNCTION [dbo].[GetPTOTimeTypeId]()
RETURNS INT
AS
BEGIN
	DECLARE @Id	INT

	SELECT @Id = TimeTypeId
	FROM TimeType
	WHERE Code = 'W9310'--Here 'W9310' is the code of PTO work type.

	RETURN @Id
END
