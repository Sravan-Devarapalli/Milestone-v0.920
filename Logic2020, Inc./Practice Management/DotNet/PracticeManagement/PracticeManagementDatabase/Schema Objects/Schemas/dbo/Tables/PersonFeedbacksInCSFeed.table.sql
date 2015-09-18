CREATE TABLE [dbo].[PersonFeedbacksInCSFeed]
(
	Id			INT		IDENTITY(1,1) NOT NULL,
	PersonId	INT		NOT NULL,
	ReviewStartDate	DATETIME NOT NULL,
	ReviewEndDate	DATETIME NOT NULL,
	ProjectId	INT		NOT NULL,
	Count		INT		NOT NULL
	CONSTRAINT PK_PersonFeedbacksInCSFeed PRIMARY KEY (Id)
)

