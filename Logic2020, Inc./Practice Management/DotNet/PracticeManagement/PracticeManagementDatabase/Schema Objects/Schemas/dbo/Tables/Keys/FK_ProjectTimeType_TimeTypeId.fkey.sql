ALTER TABLE [dbo].[ProjectTimeType]
	ADD CONSTRAINT [FK_ProjectTimeType_TimeTypeId] 
	FOREIGN KEY ([TimeTypeId]) 
	REFERENCES dbo.TimeType(TimeTypeId) ON DELETE NO ACTION ON UPDATE NO ACTION;
