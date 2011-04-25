ALTER TABLE [dbo].[Client]
    ADD CONSTRAINT [FK_Client_DefaultDirectorId] FOREIGN KEY ([DefaultDirectorID]) 
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
