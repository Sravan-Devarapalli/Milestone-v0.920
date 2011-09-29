CREATE TABLE [dbo].[PersonPractice](
	[PracticeId] int NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_PersonPractice] PRIMARY KEY CLUSTERED 
(
	[PracticeId] ASC,
	[PersonId] ASC
)
)
