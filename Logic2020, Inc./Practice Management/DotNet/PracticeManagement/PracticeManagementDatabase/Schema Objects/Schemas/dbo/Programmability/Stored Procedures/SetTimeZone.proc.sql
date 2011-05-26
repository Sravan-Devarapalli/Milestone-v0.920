CREATE PROCEDURE [dbo].[SetTimeZone]
(
@GMT NVARCHAR(10)
)
AS
	IF EXISTS (SELECT id FROM TimeZones WHERE GMT = @GMT)
	BEGIN
		UPDATE TimeZones
		SET IsActive = 0 -- Deactive previous Time zone.
	
		UPDATE TimeZones
		SET IsActive = 1
		WHERE GMT = @GMT -- Active selected Time zone.
		
		UPDATE Settings
		SET Value = @GMT
		WHERE SettingsKey = 'TimeZone'
	END
