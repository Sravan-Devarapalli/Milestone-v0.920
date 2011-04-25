ALTER TABLE [dbo].[MonthlyExpense]
    ADD CONSTRAINT [FK_MonthlyExpense_WeekPaidOptionId] FOREIGN KEY ([WeekPaidOptionId]) REFERENCES [dbo].[WeekPaidOption] ([WeekPaidOptionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


