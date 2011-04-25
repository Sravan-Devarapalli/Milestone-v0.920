CREATE TABLE dbo.SettingsType
(
	[TypeId]   INT IDENTITY(1,1) NOT NULL,
	[Description] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_SettingsType_TypeId PRIMARY KEY CLUSTERED([TypeId])
); 
