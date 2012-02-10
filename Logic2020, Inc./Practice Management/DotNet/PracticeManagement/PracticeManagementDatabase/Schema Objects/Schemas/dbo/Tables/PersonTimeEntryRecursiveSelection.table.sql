CREATE TABLE [dbo].[PersonTimeEntryRecursiveSelection]
(
	Id					INT IDENTITY (1, 1) NOT NULL,
	PersonId			INT NOT NULL,
	ClientId			INT NOT NULL,
	ProjectId			INT NOT NULL,
	ProjectGroupId		INT NOT NULL,
	TimeEntrySectionId	INT NOT NULL,
	StartDate			DATETIME NOT NULL,
	EndDate				DATETIME NULL,
	IsRecursive			BIT NOT NULL CONSTRAINT DF_PersonTimeEntryRecursiveSelection_IsRecursive DEFAULT 0,
	CONSTRAINT PK_PersonTimeEntryRecursiveSelection_Id PRIMARY KEY CLUSTERED (Id ASC)
);

