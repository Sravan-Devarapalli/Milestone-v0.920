ALTER TABLE [dbo].[ProjectStatusHistory]
	ADD CONSTRAINT [FK_ProjectStatusHistory_ProjectStatusId] 
	FOREIGN KEY ([ProjectStatusId])
	REFERENCES [dbo].[ProjectStatus] ([ProjectStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION

	
