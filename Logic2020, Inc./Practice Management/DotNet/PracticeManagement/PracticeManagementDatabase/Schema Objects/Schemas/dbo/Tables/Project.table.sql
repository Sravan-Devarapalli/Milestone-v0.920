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
    [IsChargeable]     BIT             NULL,
	[DirectorId]	   INT			   NULL,
	[Description]      NVARCHAR (MAX)  NULL,
	[CanCreateCustomWorkTypes]	BIT	   NOT NULL,
	[IsInternal]	            BIT	   NOT NULL CONSTRAINT DF_Project_IsInternal DEFAULT 0,--added as internal client can be having external projects.
	[IsAllowedToShow]			BIT	   NOT NULL CONSTRAINT DF_Project_IsAllowedToShow DEFAULT 1,--For not showing internal projects(like PTO,HOL,etc)/"Business Development" project in overall PM site.
	[IsAdministrative]          BIT    NOT NULL CONSTRAINT [DF_Project_IsAdministrative] DEFAULT 0,
	[IsNoteRequired]            BIT    NOT NULL CONSTRAINT DF_Project_IsNoteRequired DEFAULT (1),
	[ProjectOwnerId]            INT NULL,
	[SowBudget]					DECIMAL(18,2) NULL,
	[PricingListId]				INT NULL,
	[BusinessTypeId]			INT NULL,
	[ReviewerId]				INT NULL,
	SeniorManagerId				INT NULL,
    FOREIGN KEY ([GroupId]) REFERENCES [dbo].[ProjectGroup] ([GroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

