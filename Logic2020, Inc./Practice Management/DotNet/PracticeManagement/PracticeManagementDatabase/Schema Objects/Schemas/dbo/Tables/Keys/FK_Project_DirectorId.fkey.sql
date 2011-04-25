ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_DirectorId] FOREIGN KEY ([DirectorID]) 
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
