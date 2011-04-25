-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-24-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-12-2008
-- Description:	Gets a future date.
-- =============================================
CREATE FUNCTION [dbo].[GetFutureDate]()
RETURNS DATETIME
WITH SCHEMABINDING
AS
BEGIN
	RETURN '2029-12-31'
END

