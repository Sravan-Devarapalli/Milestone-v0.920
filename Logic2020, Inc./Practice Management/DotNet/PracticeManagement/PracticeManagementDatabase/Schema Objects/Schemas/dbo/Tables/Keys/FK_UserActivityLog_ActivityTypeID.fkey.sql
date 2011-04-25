ALTER TABLE [dbo].[UserActivityLog]
    ADD CONSTRAINT [FK_UserActivityLog_ActivityTypeID] FOREIGN KEY ([ActivityTypeID]) REFERENCES [dbo].[UserActivityType] ([ActivityTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


