CREATE TABLE [dbo].[MilestonePersonEntry] (
    [Id]                INT IDENTITY(1,1) NOT NULL,
    [MilestonePersonId] INT             NOT NULL,
    [StartDate]         DATETIME        NOT NULL,
    [EndDate]           DATETIME        NULL,
    [PersonRoleId]      INT             NULL,
    [Amount]            DECIMAL (18, 2) NULL,
	[Location]			NVARCHAR(20)    NULL,
    [HoursPerDay]       DECIMAL (4, 2)  NOT NULL,
	IsBadgeRequired		BIT				NULL,
	BadgeStartDate		DATETIME		NULL,
	BadgeEndDate		DATETIME		NULL,
	IsBadgeException	BIT				NULL,
	IsApproved			BIT				NULL,
	BadgeRequestDate	DATETIME		NULL,
	Requester			INT				NULL
);

 

