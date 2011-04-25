CREATE TABLE [dbo].[TimeEntries] (
    [TimeEntryId]       INT            IDENTITY (1, 1) NOT NULL,
    [EntryDate]         DATETIME       NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    [MilestonePersonId] INT            NOT NULL,
    [ActualHours]       REAL           NOT NULL,
    [ForecastedHours]   REAL           NOT NULL,
    [TimeTypeId]        INT            NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [Note]              VARCHAR (1000) NOT NULL,
    [IsReviewed]        BIT            NULL,
    [MilestoneDate]     DATETIME       NOT NULL,
    [IsChargeable]      BIT            NOT NULL,
    [IsCorrect]         BIT            NOT NULL
);


