CREATE TABLE [dbo].[MonthlyExpense] (
    [MonthlyExpenseId]  INT             IDENTITY (1, 1) NOT NULL,
    [ExpenseCategoryId] INT             NOT NULL,
    [ExpenseBasisId]    INT             NOT NULL,
    [WeekPaidOptionId]  INT             NOT NULL,
    [Name]              NVARCHAR (50)   NOT NULL,
    [Year]              INT             NOT NULL,
    [Month]             INT             NOT NULL,
    [Amount]            DECIMAL (18, 3) NOT NULL
);


