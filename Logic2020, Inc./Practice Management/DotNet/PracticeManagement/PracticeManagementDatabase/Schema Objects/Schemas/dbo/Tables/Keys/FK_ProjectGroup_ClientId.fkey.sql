ALTER TABLE [dbo].[ProjectGroup]
	ADD CONSTRAINT [FK_ProjectGroup_ClientId] 
	FOREIGN KEY ([ClientId]) 
	REFERENCES [dbo].[Client] ([ClientId]) ON DELETE NO ACTION ON UPDATE NO ACTION
