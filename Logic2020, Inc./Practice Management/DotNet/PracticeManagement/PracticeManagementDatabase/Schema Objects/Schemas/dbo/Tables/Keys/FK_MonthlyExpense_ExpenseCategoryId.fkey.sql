ALTER TABLE [dbo].[MonthlyExpense]
    ADD CONSTRAINT [FK_MonthlyExpense_ExpenseCategoryId] FOREIGN KEY ([ExpenseCategoryId]) REFERENCES [dbo].[ExpenseCategory] ([ExpenseCategoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


