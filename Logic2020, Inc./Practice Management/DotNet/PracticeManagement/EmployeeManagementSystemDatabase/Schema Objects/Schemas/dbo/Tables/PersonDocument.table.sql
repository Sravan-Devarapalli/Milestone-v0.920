CREATE TABLE [dbo].[PersonDocument](
	[DocumentTypeId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC,
	[PersonId] ASC
)
)
