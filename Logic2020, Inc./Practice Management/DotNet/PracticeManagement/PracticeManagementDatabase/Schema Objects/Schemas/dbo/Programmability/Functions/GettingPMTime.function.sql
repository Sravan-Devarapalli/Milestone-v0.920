CREATE FUNCTION [dbo].[GettingPMTime]
(
	@utcDateTime DateTime 
)
RETURNS DATETIME
AS
BEGIN
	DECLARE @currentTimeZone NVARCHAR(10)
	SET @currentTimeZone = (SELECT Value FROM Settings WHERE SettingsKey = 'TimeZone')
	
	DECLARE @resultTime DATETIME
	SET @resultTime = CONVERT(DATETIME, SWITCHOFFSET(CONVERT(DATETIMEOFFSET, @utcDateTime), @currentTimeZone))
	
	RETURN @resultTime
END
