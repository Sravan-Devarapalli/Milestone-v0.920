ALTER TABLE [dbo].[TimeEntries]
    ADD CONSTRAINT [FK_TimeEntries_MilestonePerson] FOREIGN KEY ([MilestonePersonId]) REFERENCES [dbo].[MilestonePerson] ([MilestonePersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


