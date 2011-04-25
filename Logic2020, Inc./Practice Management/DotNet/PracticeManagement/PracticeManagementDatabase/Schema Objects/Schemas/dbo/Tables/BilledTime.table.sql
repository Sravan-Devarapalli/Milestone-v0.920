CREATE TABLE [dbo].[BilledTime] (
    [PersonId]     INT             NOT NULL,
    [MilestoneId]  INT             NOT NULL,
    [BilledTimeId] INT             NOT NULL,
    [BilledDate]   DATETIME        NOT NULL,
    [Hours]        DECIMAL (18, 2) NOT NULL
);


