CREATE TABLE [Skills].[PersonDocument](
	[DocumentTypeId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonDocument] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[DocumentTypeId] ASC,
	[PersonId] ASC
)
)
