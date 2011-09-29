CREATE TABLE [dbo].[DocumentType](
	[DocumentTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)
)
