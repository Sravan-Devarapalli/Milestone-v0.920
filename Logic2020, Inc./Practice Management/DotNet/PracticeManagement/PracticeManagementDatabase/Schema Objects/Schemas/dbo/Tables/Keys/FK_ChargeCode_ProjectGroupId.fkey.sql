ALTER TABLE [dbo].[ChargeCode]
	ADD CONSTRAINT [FK_ChargeCode_ProjectGroupId] 
	FOREIGN KEY (ProjectGroupId)
	REFERENCES dbo.ProjectGroup (GroupId)  ON DELETE NO ACTION ON UPDATE NO ACTION;
