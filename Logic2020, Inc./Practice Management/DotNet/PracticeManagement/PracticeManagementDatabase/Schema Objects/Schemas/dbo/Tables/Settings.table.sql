CREATE TABLE dbo.Settings (
    Id                    INT IDENTITY(1,1) NOT NULL,
    TypeId				  INT NOT NULL,
    SettingsKey          NVARCHAR(255) NOT NULL,
    Value                 NVARCHAR(MAX) NOT NULL,
   CONSTRAINT PK_Settings_SettingsKey PRIMARY KEY CLUSTERED(SettingsKey),
   CONSTRAINT FK_Settings_TypeId FOREIGN KEY(TypeId) REFERENCES  dbo.SettingsType(TypeId)
);
