ALTER TABLE [dbo].[TimeEntryHistory]  
WITH NOCHECK ADD  CONSTRAINT [FK_TimeEntryHistory_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
