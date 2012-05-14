ALTER TABLE [dbo].[TimeEntryHours]  
WITH CHECK ADD CONSTRAINT [FK_TimeEntryHours_ReviewStatusId] FOREIGN KEY(ReviewStatusId)
REFERENCES dbo.TimeEntryReviewStatus (Id)
