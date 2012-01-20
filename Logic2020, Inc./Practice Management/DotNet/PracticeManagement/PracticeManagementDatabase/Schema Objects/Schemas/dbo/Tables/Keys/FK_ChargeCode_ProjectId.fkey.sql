ALTER TABLE [dbo].[ChargeCode]
	ADD CONSTRAINT [FK_ChargeCode_ProjectId] 
	FOREIGN KEY (ProjectId) 
	REFERENCES dbo.Project (ProjectId) ON DELETE NO ACTION ON UPDATE NO ACTION;
