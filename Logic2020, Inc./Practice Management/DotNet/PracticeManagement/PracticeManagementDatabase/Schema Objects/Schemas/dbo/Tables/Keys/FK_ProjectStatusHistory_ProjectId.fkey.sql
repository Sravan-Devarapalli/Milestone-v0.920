ALTER TABLE [dbo].[ProjectStatusHistory]
	ADD CONSTRAINT [FK_ProjectStatusHistory_ProjectId] 
	FOREIGN KEY ([ProjectId])
	REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION
	 


