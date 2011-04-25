--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-26-2008
-- Description:	Gets a current date without the time part.
-- =============================================
CREATE FUNCTION [dbo].[Today]()
RETURNS DATETIME
AS
BEGIN
	DECLARE @Today DATETIME
	SET @Today = GETDATE()
	SET @Today =
		CAST(CAST(YEAR(@Today) AS VARCHAR) + '-' +
			CAST(MONTH(@Today) AS VARCHAR) + '-' +
			CAST(DAY(@Today) AS VARCHAR) AS DATETIME)

	RETURN @Today
END

