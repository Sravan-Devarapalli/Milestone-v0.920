-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-28-2008
-- Updated by:  
-- Update date: 
-- Description:	Creates a date from its parts.
-- =============================================
CREATE FUNCTION [dbo].[MakeDate]
(
	@Year    INT,
	@Month   INT,
	@Day     INT = NULL
)
RETURNS DATETIME
AS
BEGIN
	DECLARE @Result DATETIME

	IF @Day IS NULL
	BEGIN
		SET @Result = dbo.MakeDate(@Year, @Month, dbo.GetDaysInMonth(dbo.MakeDate(@Year, @Month, 1)))
	END
	ELSE
	BEGIN
		SET @Result = CAST(CAST(@Year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-' + CAST(@Day AS VARCHAR) AS DATETIME)
	END

	RETURN @Result
END

