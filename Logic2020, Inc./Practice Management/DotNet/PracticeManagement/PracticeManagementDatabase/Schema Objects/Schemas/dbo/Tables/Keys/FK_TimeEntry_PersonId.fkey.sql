ALTER TABLE [dbo].[TimeEntry]
	ADD CONSTRAINT [FK_TimeEntry_PersonId] 
	FOREIGN KEY (PersonId)
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
