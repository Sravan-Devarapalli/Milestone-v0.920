ALTER TABLE [dbo].[ProjectTimeType]
	ADD CONSTRAINT [FK_ProjectTimeType_ProjectId] 
	FOREIGN KEY ([ProjectId]) 
	REFERENCES dbo.Project (ProjectId) ON DELETE NO ACTION ON UPDATE NO ACTION;
