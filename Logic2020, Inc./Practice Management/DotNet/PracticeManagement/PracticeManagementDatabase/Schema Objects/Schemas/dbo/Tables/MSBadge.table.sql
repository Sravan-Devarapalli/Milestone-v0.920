CREATE TABLE [dbo].[MSBadge]
(
	PersonId			INT			NOT NULL,
	BadgeStartDate		DATETIME	NULL,
	BadgeEndDate		DATETIME	NULL,
	PlannedEndDate		DATETIME	NULL,
	BreakStartDate		DATETIME	NULL,
	BreakEndDate		DATETIME	NULL,
	IsBlocked			BIT			NOT NULL,
	BlockStartDate		DATETIME	NULL,
	BlockEndDate		DATETIME	NULL,
	IsPreviousBadge		BIT			NOT NULL,
	PreviousBadgeAlias	NVARCHAR(20) NULL,
	LastBadgeStartDate	DATETIME	NULL,
	LastBadgeEndDate	DATETIME	NULL,
	IsException			BIT			NOT NULL,
	ExceptionStartDate	DATETIME	NULL,
	ExceptionEndDate	DATETIME	NULL,
	BadgeStartDateSource NVARCHAR(30) NULL,
	PlannedEndDateSource NVARCHAR(30) NULL,
	BadgeEndDateSource NVARCHAR(30) NULL
)

