CREATE TABLE [dbo].[Calendar] (
    [Date]					DATETIME NOT NULL,
    [DayOff]				BIT      NOT NULL,
	[IsRecurring]			BIT NULL,
	[RecurringHolidayId]	INT NULL,
	[HolidayDescription]	NVARCHAR(255),
	[RecurringHolidayDate]  DATETIME NULL
);
