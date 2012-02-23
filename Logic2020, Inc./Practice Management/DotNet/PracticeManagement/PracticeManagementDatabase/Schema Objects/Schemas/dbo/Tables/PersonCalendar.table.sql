CREATE TABLE [dbo].[PersonCalendar] (
    [Date]				DATETIME	NOT NULL,
    [PersonId]			INT			NOT NULL,
    [DayOff]			BIT			NOT NULL,
	[ActualHours]		REAL		NULL,
	[TimeTypeId]		INT			NULL,
	[IsFromTimeEntry]	BIT			NULL,
	[SubstituteDate]	DATETIME	NULL,
	[IsFloatingHoliday] BIT NULL,
	[Description]		NVARCHAR(500) NULL
);


