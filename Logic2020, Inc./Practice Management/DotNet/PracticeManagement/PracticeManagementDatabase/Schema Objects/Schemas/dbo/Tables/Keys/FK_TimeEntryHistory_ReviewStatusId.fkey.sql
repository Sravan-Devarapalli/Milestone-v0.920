ALTER TABLE [dbo].[TimeEntryHistory]  
WITH CHECK ADD CONSTRAINT [FK_TimeEntryHistory_ReviewStatusId] FOREIGN KEY(ReviewStatusId)
REFERENCES dbo.TimeEntryReviewStatus (Id)
