CREATE TABLE dbo.PracticeStatusHistory
(
	PracticeId		INT NOT NULL,
	IsActive		BIT NOT NULL,
	StartDate		DATETIME NOT NULL,
	EndDate			DATETIME NULL,
	CONSTRAINT PK_PracticeStatusHistory PRIMARY KEY CLUSTERED
	(
		PracticeId ASC,	
		IsActive   ASC,	
		StartDate  ASC
	),
	CONSTRAINT FK_PracticeStatusHistory_PracticeId FOREIGN KEY (PracticeId)
	REFERENCES dbo.Practice(PracticeId)
)
