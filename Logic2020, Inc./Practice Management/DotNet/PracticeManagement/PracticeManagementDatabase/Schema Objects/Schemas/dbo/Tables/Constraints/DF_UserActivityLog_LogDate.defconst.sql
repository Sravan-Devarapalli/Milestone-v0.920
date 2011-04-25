ALTER TABLE [dbo].[UserActivityLog]
    ADD CONSTRAINT [DF_UserActivityLog_LogDate] DEFAULT (getdate()) FOR [LogDate];


