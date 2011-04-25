--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-19-2008
-- Description:	Gets a number of days in the month
-- =============================================
CREATE FUNCTION [dbo].[GetDaysInMonth]
(
	@date DATETIME
)
RETURNS INT
AS
BEGIN
	DECLARE @year INT
	DECLARE @month INT

	SET @year = YEAR(@date)
	SET @month = MONTH(@date)

	RETURN
		CASE
			WHEN @month IN (1, 3, 5, 7, 8, 10, 12) THEN 31
			WHEN @month IN (4, 6, 9, 11) THEN 30
			ELSE
				CASE
					WHEN (@year % 4 = 0 AND @year % 100 != 0) OR (@year % 400 = 0)
					THEN 29
					ELSE 28
				END
		END
END

