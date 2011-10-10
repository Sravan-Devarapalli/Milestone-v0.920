CREATE TABLE [Skills].[PersonDocument](
	[DocumentTypeId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC,
	[PersonId] ASC
)
)
