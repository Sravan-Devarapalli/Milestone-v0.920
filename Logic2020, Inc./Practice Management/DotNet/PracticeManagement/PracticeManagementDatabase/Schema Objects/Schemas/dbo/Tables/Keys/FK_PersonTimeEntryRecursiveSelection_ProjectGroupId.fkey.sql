ALTER TABLE [dbo].[PersonTimeEntryRecursiveSelection]
	ADD CONSTRAINT [FK_PersonTimeEntryRecursiveSelection_ProjectGroupId] 
	FOREIGN KEY (ProjectGroupId)
	REFERENCES [dbo].[ProjectGroup] ([GroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
