CREATE TABLE [Skills].[Practice](
	[PracticeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_Practice] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[PracticeId] ASC
)
)
