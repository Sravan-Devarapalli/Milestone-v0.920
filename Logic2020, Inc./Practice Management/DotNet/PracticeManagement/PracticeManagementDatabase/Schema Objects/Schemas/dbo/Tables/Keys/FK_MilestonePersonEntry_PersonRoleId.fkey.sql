ALTER TABLE [dbo].[MilestonePersonEntry]
    ADD CONSTRAINT [FK_MilestonePersonEntry_PersonRoleId] FOREIGN KEY ([PersonRoleId]) REFERENCES [dbo].[PersonRole] ([PersonRoleId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


