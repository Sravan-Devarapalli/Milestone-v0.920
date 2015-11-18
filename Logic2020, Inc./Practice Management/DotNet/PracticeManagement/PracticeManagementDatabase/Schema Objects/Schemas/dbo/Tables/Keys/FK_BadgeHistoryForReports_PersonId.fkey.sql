ALTER TABLE [dbo].[BadgeHistoryForReports]
	ADD CONSTRAINT [FK_BadgeHistoryForReports_PersonId] 
	FOREIGN KEY (PersonId)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


