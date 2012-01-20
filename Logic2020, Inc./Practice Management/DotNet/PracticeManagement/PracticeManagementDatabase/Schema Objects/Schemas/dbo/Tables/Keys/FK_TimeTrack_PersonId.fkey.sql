ALTER TABLE [dbo].[TimeTrack]
	ADD CONSTRAINT [FK_TimeTrack_PersonId] 
	FOREIGN KEY ([PersonId]) 
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
