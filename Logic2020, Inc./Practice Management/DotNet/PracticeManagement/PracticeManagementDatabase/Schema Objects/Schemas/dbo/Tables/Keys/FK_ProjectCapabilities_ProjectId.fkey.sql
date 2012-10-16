ALTER TABLE [dbo].[ProjectCapabilities]
ADD CONSTRAINT [FK_ProjectCapabilities_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project]([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
