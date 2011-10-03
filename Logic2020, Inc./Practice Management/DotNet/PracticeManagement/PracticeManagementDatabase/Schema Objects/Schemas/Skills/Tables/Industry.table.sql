CREATE TABLE [Skills].[Industry](
	[IndustryId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_Industry] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[IndustryId] ASC
)
)
