CREATE TABLE [dbo].[BadgeHistory]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[PersonId]			INT         NOT NULL,
	BadgeStartDate		DATETIME	NULL,
	BadgeEndDate		DATETIME	NULL,
	ProjectPlannedEndDate		DATETIME	NULL,
	BreakStartDate		DATETIME	NULL,
	BreakEndDate		DATETIME	NULL,
	BadgeStartDateSource	NVARCHAR(30) NULL,
	ProjectPlannedEndDateSource	NVARCHAR(30) NULL,
	BadgeEndDateSource NVARCHAR(30) NULL,
	ModifiedDate		DATETIME	NOT NULL,
	ModifiedBy			INT			NULL,
	DeactivatedDate		DATETIME	NULL,
	OrganicBreakStartDate	DATETIME	NULL,
	OrganicBreakEndDate		DATETIME	NULL,
	ExcludeInReports	BIT			NULL
	CONSTRAINT PK_BadgeHistory PRIMARY KEY (Id)
)

