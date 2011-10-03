CREATE TABLE [Skills].[Title](
	[TitleId] INT NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_Title] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[TitleId] ASC
)
)
