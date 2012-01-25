CREATE TABLE [dbo].[Project] (
    [ClientId]         INT             NULL,
    [ProjectId]        INT             IDENTITY (1, 1) NOT NULL,
    [Discount]         DECIMAL (18, 2) NOT NULL,
    [Terms]            INT             NOT NULL,
    [Name]             NVARCHAR (100)  NOT NULL,
    [PracticeId]       INT             NOT NULL,
    [StartDate]        DATETIME        NULL,
    [EndDate]          DATETIME        NULL,
    [ProjectStatusId]  INT             NOT NULL,
    [ProjectNumber]    NVARCHAR (12)   NOT NULL,
    [BuyerName]        NVARCHAR (100)  NULL,
    [OpportunityId]    INT             NULL,
    [GroupId]          INT             NULL,
    [IsChargeable]     BIT             NOT NULL,
	[DirectorId]	   INT			   NULL,
	[Description]      NVARCHAR (MAX)  NULL,
	[CanCreateCustomWorkTypes]	BIT	   NOT NULL,
	[IsInternal]	   BIT			   NOT NULL DEFAULT 0,--added as internal client can be having external projects.
	[IsAllowedToShow]			BIT	   NOT NULL DEFAULT 1--For not showing internal projects(like PTO,HOL,etc)/"Business Development" project in overall PM site.
    FOREIGN KEY ([GroupId]) REFERENCES [dbo].[ProjectGroup] ([GroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
