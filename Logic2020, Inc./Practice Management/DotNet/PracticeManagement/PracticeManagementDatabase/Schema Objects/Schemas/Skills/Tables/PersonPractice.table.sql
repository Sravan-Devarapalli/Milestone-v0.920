CREATE TABLE [Skills].[PersonPractice](
	[PracticeId] int NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonPractice] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[PracticeId] ASC,
	[PersonId] ASC
)
)
