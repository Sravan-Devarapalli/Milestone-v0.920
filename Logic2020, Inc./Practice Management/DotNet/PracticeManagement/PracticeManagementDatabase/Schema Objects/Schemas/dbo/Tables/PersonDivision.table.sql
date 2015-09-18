CREATE TABLE [dbo].[PersonDivision]
(
	[DivisionId]	INT IDENTITY(1,1) NOT NULL,
	[DivisionName]	NVARCHAR(100) NOT NULL,
	[PracticeDirectorId]	INT	NULL
	CONSTRAINT [PK_PersonDivision_DivisionId]	PRIMARY KEY ([DivisionId])
)

