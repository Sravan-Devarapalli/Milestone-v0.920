CREATE TABLE [Skills].[DocumentType](
	[DocumentTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[DocumentTypeId] ASC
)
)
