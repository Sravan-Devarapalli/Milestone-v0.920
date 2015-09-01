ALTER TABLE [dbo].[PracticeLeadership]
	ADD CONSTRAINT [FK_PracticeLeadership_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person](PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


