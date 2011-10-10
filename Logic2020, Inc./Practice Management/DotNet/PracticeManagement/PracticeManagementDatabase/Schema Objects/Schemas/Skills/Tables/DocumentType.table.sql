CREATE TABLE [Skills].[DocumentType](
	[DocumentTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_DocumentType_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_DocumentType_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)
)
