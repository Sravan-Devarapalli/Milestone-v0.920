CREATE TABLE [dbo].[PersonCalendar] (
    [Date]				DATETIME	NOT NULL,
    [PersonId]			INT			NOT NULL,
    [DayOff]			BIT			NOT NULL,
	[ActualHours]		REAL		NULL,
	[IsSeries]			BIT NOT NULL CONSTRAINT DF_PersonCalendar_IsSeries DEFAULT 0,
	[TimeTypeId]		INT NULL,
	[SubstituteDate]	DATETIME NULL,
	[Description]		NVARCHAR(500) NULL,
	[IsFromTimeEntry]	BIT NULL,
	[ApprovedBy]		INT NULL,
	[SeriesId]          BIGINT NOT NULL CONSTRAINT DF_PersonCalendar_SeriesId DEFAULT 0
);

