ALTER TABLE [dbo].[Attribution]
	ADD CONSTRAINT [FK_Attribution_ProjectId] 
	FOREIGN KEY (ProjectId) 
	REFERENCES dbo.Project (ProjectId) ON DELETE NO ACTION ON UPDATE NO ACTION;


