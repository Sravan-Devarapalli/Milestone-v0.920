CREATE TABLE [dbo].[BadgeHistoryForReports]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[PersonId]			INT         NOT NULL,
	BadgeStartDate		DATETIME	NULL,
	BadgeEndDate		DATETIME	NULL,
	ProjectPlannedEndDate		DATETIME	NULL,
	BreakStartDate		DATETIME	NULL,
	BreakEndDate		DATETIME	NULL,
	BadgeStartDateSource NVARCHAR(30) NULL,
	PlannedEndDateSource NVARCHAR(30) NULL,
	BadgeEndDateSource NVARCHAR(30) NULL,
	CONSTRAINT PK_BadgeHistoryForReports PRIMARY KEY (Id)
)

