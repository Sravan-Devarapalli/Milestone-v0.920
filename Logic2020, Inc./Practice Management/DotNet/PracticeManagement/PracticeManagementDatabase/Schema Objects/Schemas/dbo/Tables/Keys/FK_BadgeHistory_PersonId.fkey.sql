ALTER TABLE [dbo].[BadgeHistory]
	ADD CONSTRAINT [FK_BadgeHistory_PersonId] 
	FOREIGN KEY (PersonId)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


