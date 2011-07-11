ALTER TABLE [dbo].[ProjectExpense]  WITH CHECK ADD  CONSTRAINT [FK_ProjectExpense_ProjectId] 
FOREIGN KEY([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])

