CREATE TABLE [dbo].[PersonFeedbacksInCSFeed]
(
	Id			INT		IDENTITY(1,1) NOT NULL,
	PersonId	INT		NOT NULL,
	ReviewStartDate	DATETIME NOT NULL,
	ReviewEndDate	DATETIME NOT NULL,
	ProjectId	INT		NULL,
	Count		INT		NOT NULL,
	IsDummy		BIT		NULL,
	ManagerPaychexID INT NULL
	CONSTRAINT PK_PersonFeedbacksInCSFeed PRIMARY KEY (Id)
)

