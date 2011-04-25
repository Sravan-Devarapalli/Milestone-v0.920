ALTER TABLE [dbo].[TimeEntries]
    ADD CONSTRAINT [DF_TimeEntries_IsReviewed] DEFAULT (NULL) FOR [IsReviewed];


