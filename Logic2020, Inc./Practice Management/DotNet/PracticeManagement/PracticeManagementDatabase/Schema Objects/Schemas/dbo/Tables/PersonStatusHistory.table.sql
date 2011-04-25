CREATE TABLE dbo.PersonStatusHistory
(
	PersonId		INT NOT NULL,
	PersonStatusId	INT NOT NULL,
	StartDate		DATETIME NOT NULL,
	EndDate			DATETIME NULL,
	CONSTRAINT PK_PersonStatusHistory PRIMARY KEY CLUSTERED
	(
		PersonId ASC,	
		PersonStatusId ASC,	
		StartDate ASC
	),
	CONSTRAINT FK_PersonStatusHistory_PersonId FOREIGN KEY (PersonId)
	REFERENCES dbo.Person(PersonId),
	CONSTRAINT FK_PersonStatusHistory_StatusId FOREIGN KEY (PersonStatusId)
	REFERENCES dbo.PersonStatus(PersonStatusId)
)
