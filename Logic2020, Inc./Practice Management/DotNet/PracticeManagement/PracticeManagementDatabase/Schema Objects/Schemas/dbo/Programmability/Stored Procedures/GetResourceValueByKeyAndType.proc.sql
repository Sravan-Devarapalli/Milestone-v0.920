CREATE PROCEDURE dbo.GetResourceValueByKeyAndType
(
	@SettingType	NVARCHAR(255),
	@Key			NVARCHAR(255)
)
AS

	SELECT  [Value]
	FROM [dbo].[Settings] S
	JOIN dbo.SettingsType ST ON S.TypeId = ST.TypeId
	WHERE ST.[Description]  = @SettingType AND s.SettingsKey = @Key
