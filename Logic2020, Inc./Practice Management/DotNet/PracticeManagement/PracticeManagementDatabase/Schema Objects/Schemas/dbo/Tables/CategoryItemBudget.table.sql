CREATE TABLE [dbo].[CategoryItemBudget] (
    [ItemId]			INT NOT NULL,
    [CategoryTypeId]    INT NOT NULL,
    [MonthStartDate]	DATETIME NOT NULL,
    [Amount]			DECIMAL(18,2),
    CONSTRAINT PK_CategoryItemBudget PRIMARY KEY ([ItemId] ASC,[CategoryTypeId] ASC, [MonthStartDate] ASC),
    CONSTRAINT FK_CategoryItemBudget_CategoryType FOREIGN KEY([CategoryTypeId]) 
	REFERENCES [dbo].[BudgetCategoryType]([CategoryTypeId])
);
