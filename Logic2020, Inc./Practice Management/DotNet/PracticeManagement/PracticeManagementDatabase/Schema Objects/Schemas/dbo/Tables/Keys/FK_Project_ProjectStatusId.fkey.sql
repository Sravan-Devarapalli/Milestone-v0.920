ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_ProjectStatusId] FOREIGN KEY ([ProjectStatusId]) REFERENCES [dbo].[ProjectStatus] ([ProjectStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


