ALTER TABLE [dbo].[PersonDivision]
	ADD CONSTRAINT [FK_PersonDivision_PracticeDirectorId] FOREIGN KEY (PracticeDirectorId)	REFERENCES [dbo].[Person](PersonId)	ON DELETE NO ACTION ON UPDATE NO ACTION;


