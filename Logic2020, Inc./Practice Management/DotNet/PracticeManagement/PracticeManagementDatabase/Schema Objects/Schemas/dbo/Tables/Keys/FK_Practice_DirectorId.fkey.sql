ALTER TABLE [dbo].[Practice]
    ADD CONSTRAINT [FK_Practice_DirectorId] FOREIGN KEY ([DirectorId])
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


