CREATE TABLE [dbo].[ProjectExpense] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL,
    [Amount]        DECIMAL (18, 2) NOT NULL,
    [Reimbursement] DECIMAL (18, 2) NOT NULL,
    [ProjectId]		INT             NOT NULL,
	[StartDate]		DATETIME		NOT NULL,
	[EndDate]		DATETIME		NOT NULL
);

