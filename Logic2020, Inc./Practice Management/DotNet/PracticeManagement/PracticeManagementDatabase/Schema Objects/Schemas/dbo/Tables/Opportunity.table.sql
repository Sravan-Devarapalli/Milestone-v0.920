CREATE TABLE [dbo].[Opportunity] (
    [OpportunityId]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (50)  NOT NULL,
    [ClientId]            INT            NOT NULL,
    [SalespersonId]       INT            NULL,
    [OpportunityStatusId] INT            NOT NULL,
    [Priority]            CHAR (1)       NOT NULL,
    [ProjectedStartDate]  DATETIME       NULL,
    [ProjectedEndDate]    DATETIME       NULL,
    [OpportunityNumber]   NVARCHAR (12)  NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [PracticeId]          INT            NOT NULL,
    [BuyerName]           NVARCHAR (100) NULL,
    [CreateDate]          DATETIME       NOT NULL,
    [Pipeline]            NVARCHAR (512) NULL,
    [Proposed]            NVARCHAR (512) NULL,
    [SendOut]             NVARCHAR (512) NULL,
    [ProjectId]           INT            NULL,
    [OpportunityIndex]    INT            NULL,
    [RevenueType]         INT            NOT NULL,
    [OwnerId]             INT            NULL,
    [GroupId]             INT            NULL,
    [LastUpdated]         DATETIME       NOT NULL,
	[EstimatedRevenue]    DECIMAL(18, 2) NULL          
);
 

