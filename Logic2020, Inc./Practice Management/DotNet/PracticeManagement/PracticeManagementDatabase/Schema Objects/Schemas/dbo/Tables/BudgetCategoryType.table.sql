CREATE TABLE [dbo].[BudgetCategoryType] (
    [CategoryTypeId]   INT IDENTITY (1, 1) NOT NULL,
    [Description]      NVARCHAR (225) NOT NULL,
    CONSTRAINT PK_BudgetCategoryType PRIMARY KEY ([CategoryTypeId] ASC)
);
GO

