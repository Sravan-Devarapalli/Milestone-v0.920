CREATE PROCEDURE dbo.SaveSettingsKeyValuePairs
(
	@TypeId   INT,
	@SettingsKey NVARCHAR(255),
	@Value      NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON
	IF NOT EXISTS(SELECT TOP 1 1 
				  FROM [dbo].[Settings]
				  WHERE @SettingsKey = SettingsKey 
				  AND TypeId = @TypeId 
				 )
	BEGIN
		INSERT INTO [dbo].[Settings]
					([TypeId]
					,[SettingsKey]
					,[Value])
		 VALUES
					(@TypeId
					,@SettingsKey
					,@Value)
           
	END
	ELSE 
	BEGIN
		UPDATE [dbo].[Settings]
		SET  Value = @Value
		WHERE SettingsKey = @SettingsKey  AND TypeId = @TypeId
	END
END
           
