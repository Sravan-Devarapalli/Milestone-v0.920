﻿CREATE TABLE [dbo].[TimeEntry]
(
    [TimeEntryId]       INT		IDENTITY (1, 1) NOT NULL,
    [PersonId]			INT	    NOT NULL,
    [ChargeCodeId]      INT		NOT NULL,
    [ChargeCodeDate]	DATETIME	NOT NULL,
    [Note]              VARCHAR (1000)	NOT NULL,
    [ForecastedHours]	REAL	NOT NULL,
    [IsCorrect]         BIT		NOT NULL,
    [IsAutoGenerated]	BIT 	NOT NULL,
	CONSTRAINT PK_TimeEntry_TimeEntryId PRIMARY KEY CLUSTERED (TimeEntryId ASC)
)

