ALTER TABLE [dbo].[TimeEntryHours]  
WITH CHECK ADD  CONSTRAINT [FK_TimeEntryHours_ModifiedBy] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[Person] ([PersonId])
