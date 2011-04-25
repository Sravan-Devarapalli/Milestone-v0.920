CREATE TABLE [dbo].[CompanyLogoSetting](
	[Id]        [int] IDENTITY(1,1) PRIMARY KEY  NOT NULL,
	[Title]     [nvarchar](256)      NOT NULL,
	[FileName]  [nvarchar](256)      NULL,
	[FilePath]  [nvarchar](1024)     NULL,
	[Data]      [varbinary](max)     NULL
)

