CREATE TABLE [dbo].[PracticeLeadership]
(
	[PracticeLeadershipId]	INT IDENTITY(1,1) NOT NULL, 
	[DivisionId]			INT	NOT NULL,
	[PersonId]				INT NOT NULL
	CONSTRAINT [PK_PracticeLeadership_PracticeLeadershipId]	PRIMARY KEY ([PracticeLeadershipId])
)

