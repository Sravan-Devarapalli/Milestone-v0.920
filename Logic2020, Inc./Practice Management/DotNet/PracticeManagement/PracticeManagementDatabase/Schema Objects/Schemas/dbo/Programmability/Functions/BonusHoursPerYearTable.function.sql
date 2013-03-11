CREATE FUNCTION [dbo].[BonusHoursPerYearTable]()
RETURNS TABLE
AS
	RETURN 
	SELECT CONVERT(DECIMAL(10,2),s.Value) AS HoursPerYear FROM Settings s WHERE s.SettingsKey='DefaultHoursPerYear'
GO
