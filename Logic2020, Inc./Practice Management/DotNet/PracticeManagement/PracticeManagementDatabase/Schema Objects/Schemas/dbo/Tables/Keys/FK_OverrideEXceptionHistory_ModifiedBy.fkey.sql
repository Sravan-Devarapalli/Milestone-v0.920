ALTER TABLE [dbo].[OverrideExceptionHistory]
	ADD CONSTRAINT [FK_OverrideEXceptionHistory_ModifiedBy] 
	FOREIGN KEY (ModifiedBy)
	REFERENCES dbo.Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;	


