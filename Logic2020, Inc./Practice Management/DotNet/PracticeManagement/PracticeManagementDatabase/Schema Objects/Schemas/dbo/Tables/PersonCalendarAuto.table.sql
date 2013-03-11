CREATE TABLE [dbo].[PersonCalendarAuto] (
    [Date]     DATETIME NOT NULL,
    [PersonId] INT      NOT NULL,
    [DayOff]   BIT      NOT NULL,
	TimeOffHours REAL		NULL ,
	CompanyDayOff BIT	NOT	NULL
);


