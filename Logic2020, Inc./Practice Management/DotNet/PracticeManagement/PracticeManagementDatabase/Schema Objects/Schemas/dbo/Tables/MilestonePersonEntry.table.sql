CREATE TABLE [dbo].[MilestonePersonEntry] (
    [MilestonePersonId] INT             NOT NULL,
    [StartDate]         DATETIME        NOT NULL,
    [EndDate]           DATETIME        NULL,
    [PersonRoleId]      INT             NULL,
    [Amount]            DECIMAL (18, 2) NULL,
	[Location]			NVARCHAR(20)    NULL,
    [HoursPerDay]       DECIMAL (4, 2)  NOT NULL
);


