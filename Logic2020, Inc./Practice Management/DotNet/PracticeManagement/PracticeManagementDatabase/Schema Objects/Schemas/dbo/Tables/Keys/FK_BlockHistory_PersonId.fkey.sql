ALTER TABLE [dbo].[BlockHistory]
	ADD CONSTRAINT [FK_BlockHistory_PersonId] 
	FOREIGN KEY (PersonId)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


