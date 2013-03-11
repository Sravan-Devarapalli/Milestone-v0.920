CREATE TABLE [dbo].[Calendar] (
    [Date]					DATETIME NOT NULL,
    [DayOff]				BIT      NOT NULL,
	[Year]					INT	NOT NULL,
	[MonthStartDate]		DATETIME NOT NULL,
	[MonthEndDate]			DATETIME NOT NULL,
	[DaysInMonth]			INT NOT NULL,
	[DaysInYear]			INT NOT NULL,
	[IsRecurring]			BIT NULL,
	[RecurringHolidayId]	INT NULL,
	[HolidayDescription]	NVARCHAR(255),
	[RecurringHolidayDate]  DATETIME NULL
);

