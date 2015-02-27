CREATE TABLE [dbo].[BadgeHistoryForReports]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[PersonId]			INT         NOT NULL,
	BadgeStartDate		DATETIME	NULL,
	BadgeEndDate		DATETIME	NULL,
	ProjectPlannedEndDate		DATETIME	NULL,
	BreakStartDate		DATETIME	NULL,
	BreakEndDate		DATETIME	NULL,
	CONSTRAINT PK_BadgeHistoryForReports PRIMARY KEY (Id)
)

