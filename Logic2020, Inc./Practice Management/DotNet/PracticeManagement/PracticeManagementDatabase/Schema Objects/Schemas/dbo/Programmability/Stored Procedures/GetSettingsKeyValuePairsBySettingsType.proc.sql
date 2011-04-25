CREATE PROCEDURE dbo.GetSettingsKeyValuePairsBySettingsType
(
	@TypeId   INT
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT SettingsKey,
           Value
	FROM dbo.Settings
	WHERE TypeId=@TypeId
END
