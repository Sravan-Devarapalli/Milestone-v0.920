ALTER TABLE [dbo].[TimeEntryHistory]  
WITH NOCHECK ADD  CONSTRAINT [FK_TimeEntryHistory_ActivityTypeId] FOREIGN KEY([ActivityTypeId])
REFERENCES [dbo].[UserActivityType] ([ActivityTypeID])
