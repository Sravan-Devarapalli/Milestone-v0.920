ALTER TABLE [dbo].[PersonTimeEntryRecursiveSelection]
	ADD CONSTRAINT [FK_PersonTimeEntryRecursiveSelection_ClientId] 
	FOREIGN KEY (ClientId)
	REFERENCES dbo.Client (ClientId) ON DELETE NO ACTION ON UPDATE NO ACTION;
