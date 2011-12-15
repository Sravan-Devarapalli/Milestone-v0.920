﻿CREATE TABLE [dbo].[PersonCalendar] (
    [Date]     DATETIME NOT NULL,
    [PersonId] INT      NOT NULL,
    [DayOff]   BIT      NOT NULL,
	[ActualHours]		REAL NULL,
	[IsFloatingHoliday] BIT NULL,
	[IsFromTimeEntry]	BIT NULL
);


