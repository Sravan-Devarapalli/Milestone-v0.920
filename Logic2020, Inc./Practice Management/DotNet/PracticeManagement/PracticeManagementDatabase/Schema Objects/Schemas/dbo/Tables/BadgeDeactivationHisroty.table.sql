CREATE TABLE [dbo].[BadgeDeactivationHisroty]
(
	PersonId				INT			NOT NULL,
	DeactivatedDate			DATETIME	NULL,
	OrganicBreakStartDate	DATETIME	NULL,
	OrganicBreakEndDate		DATETIME	NULL,
	ManualStartDate			DATETIME	NULL,
	ManualEndDate			DATETIME	NULL,
)

