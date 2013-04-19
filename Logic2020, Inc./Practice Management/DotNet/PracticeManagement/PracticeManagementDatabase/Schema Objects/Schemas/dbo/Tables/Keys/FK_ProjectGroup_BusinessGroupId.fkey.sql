ALTER TABLE [dbo].[ProjectGroup]
    ADD CONSTRAINT [FK_ProjectGroup_BusinessGroupId] FOREIGN KEY ([BusinessGroupId]) REFERENCES [dbo].[BusinessGroup] ([BusinessGroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
