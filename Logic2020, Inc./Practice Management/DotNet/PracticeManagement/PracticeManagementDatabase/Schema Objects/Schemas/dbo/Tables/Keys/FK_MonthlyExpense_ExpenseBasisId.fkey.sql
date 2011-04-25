ALTER TABLE [dbo].[MonthlyExpense]
    ADD CONSTRAINT [FK_MonthlyExpense_ExpenseBasisId] FOREIGN KEY ([ExpenseBasisId]) REFERENCES [dbo].[ExpenseBasis] ([ExpenseBasisId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


