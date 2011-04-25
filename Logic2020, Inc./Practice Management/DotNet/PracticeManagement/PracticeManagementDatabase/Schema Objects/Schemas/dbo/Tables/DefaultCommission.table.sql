CREATE TABLE [dbo].[DefaultCommission] (
    [PersonId]         INT             NOT NULL,
    [StartDate]        SMALLDATETIME   NOT NULL,
    [EndDate]          SMALLDATETIME   NOT NULL,
    [FractionOfMargin] DECIMAL (18, 2) NOT NULL,
    [type]             INT             NOT NULL,
    [MarginTypeId]     INT             NULL
);


