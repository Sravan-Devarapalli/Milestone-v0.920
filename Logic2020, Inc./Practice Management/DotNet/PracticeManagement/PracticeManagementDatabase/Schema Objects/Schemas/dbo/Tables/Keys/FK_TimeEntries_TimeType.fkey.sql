ALTER TABLE [dbo].[TimeEntries]
    ADD CONSTRAINT [FK_TimeEntries_TimeType] FOREIGN KEY ([TimeTypeId]) REFERENCES [dbo].[TimeType] ([TimeTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


