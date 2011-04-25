CREATE TABLE [dbo].[Pay] (
    [Person]              INT             NOT NULL,
    [StartDate]           SMALLDATETIME   NOT NULL,
    [EndDate]             SMALLDATETIME   NOT NULL,
    [Amount]              DECIMAL (18, 2) NOT NULL,
    [Timescale]           INT             NOT NULL,
    [TimesPaidPerMonth]   INT             NULL,
    [Terms]               INT             NULL,
    [VacationDays]        INT             NULL,
    [BonusAmount]         DECIMAL (18, 2) NOT NULL,
    [BonusHoursToCollect] INT             NULL,
    [DefaultHoursPerDay]  DECIMAL (18, 2) NOT NULL,
	[SeniorityId]		  INT			  NULL,
	[PracticeId]		  INT			  NULL,
	[IsActivePay]		  BIT			  NULL
);


