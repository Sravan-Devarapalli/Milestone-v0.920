ALTER TABLE [dbo].[MilestonePerson]
    ADD CONSTRAINT [FK_MilestonePerson_MilestoneId] FOREIGN KEY ([MilestoneId]) REFERENCES [dbo].[Milestone] ([MilestoneId]) ON DELETE CASCADE ON UPDATE NO ACTION;


