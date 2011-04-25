ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_ProjectManager] FOREIGN KEY ([ProjectManagerId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


