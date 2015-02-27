ALTER TABLE [dbo].[BlockHistory]
	ADD CONSTRAINT [FK_BlockHistory_ModifiedBy] 
	FOREIGN KEY (ModifiedBy)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


