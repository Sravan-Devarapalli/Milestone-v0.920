CREATE TABLE [dbo].[PersonDivision]
(
	[DivisionId]	           INT IDENTITY(1,1) NOT NULL,
	[DivisionName]	           NVARCHAR(100) NOT NULL,
	[PracticeDirectorId]	   INT	NULL,
	[Inactive]                 BIT NOT NULL  CONSTRAINT DF_PersonDivision_Inactive DEFAULT(0),
	[ShowSetPracticeOwnerLink] BIT NOT NULL,
	[DivisionOwnerId]		   INT           NULL
	CONSTRAINT DF_PersonDivision_ShowSetPracticeOwnerLink DEFAULT(0)
	CONSTRAINT [PK_PersonDivision_DivisionId]	PRIMARY KEY ([DivisionId])
)

