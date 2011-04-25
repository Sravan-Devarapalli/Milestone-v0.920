-- =============================================
-- Author:		Alexey Zvekov
-- Create date: 8-1-2008
-- Updated by:  
-- Update date: 
-- Description:	Creates a specific number from date.
-- =============================================
CREATE FUNCTION [dbo].[MakeNumberFromDate]
(
    @cod CHAR,
	@date DATETIME		
)
RETURNS VARCHAR(5)
AS
BEGIN
    DECLARE @year INT
	DECLARE @month INT
    DECLARE @StringMonth VARCHAR(2)

    SET @year = YEAR(@date)
	SET @month = MONTH(@date)
    
    SET @StringMonth = CAST(DATEPART(mm, @date) AS VARCHAR(2))
    IF LEN ( @StringMonth ) = 1
			SET @StringMonth =  '0' + @StringMonth

    RETURN @cod + @StringMonth + SUBSTRING(CAST(DATEPART(yy, @date) AS VARCHAR), 3 , 2)
	 
END

