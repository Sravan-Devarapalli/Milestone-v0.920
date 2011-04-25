-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-26-2008
-- Description:	Gets a number of hours pe year
-- =============================================
CREATE FUNCTION [dbo].[GetHoursPerYear]()
RETURNS DECIMAL
WITH SCHEMABINDING
AS
BEGIN
	RETURN 2080;
END

