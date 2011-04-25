CREATE TABLE [dbo].[Project] (
    [ClientId]         INT             NOT NULL,
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
    [ProjectManagerId] INT             NOT NULL,
	[DirectorId]	   INT			   NULL,
    FOREIGN KEY ([GroupId]) REFERENCES [dbo].[ProjectGroup] ([GroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


