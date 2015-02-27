ALTER TABLE [dbo].[BadgeHistory]
	ADD CONSTRAINT [FK_BadgeHistory_ModifiedBy] 
	FOREIGN KEY (ModifiedBy)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


