ALTER TABLE [dbo].[PersonTimeEntryRecursiveSelection]
	ADD CONSTRAINT [FK_PersonTimeEntryRecursiveSelection_PersonId] 
	FOREIGN KEY (PersonId) 
	REFERENCES dbo.Person(PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;
