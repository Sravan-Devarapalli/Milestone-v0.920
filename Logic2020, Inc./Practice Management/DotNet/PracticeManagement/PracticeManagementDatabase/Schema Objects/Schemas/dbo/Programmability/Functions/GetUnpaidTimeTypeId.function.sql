-- =============================================
-- Author:	Sainath C
-- Create date: 01-06-2012
-- =============================================
CREATE FUNCTION [dbo].[GetUnpaidTimeTypeId]()
RETURNS INT
AS
BEGIN
	DECLARE @Id	INT

	SELECT @Id = TT.TimeTypeId
	FROM dbo.TimeType TT
	WHERE 
	TT.Code = 'W9350'--Here 'W9350' is the code of Unpaid work type.
	RETURN @Id
END
