ALTER TABLE [dbo].[TimeEntryHistory]  
WITH CHECK ADD  CONSTRAINT [FK_TimeEntryHistory_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[Person] ([PersonId])
