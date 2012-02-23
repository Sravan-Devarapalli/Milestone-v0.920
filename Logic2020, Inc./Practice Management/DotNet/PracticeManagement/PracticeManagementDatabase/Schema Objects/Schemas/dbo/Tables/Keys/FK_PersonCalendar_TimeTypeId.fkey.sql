ALTER TABLE [dbo].[PersonCalendar]
	ADD CONSTRAINT [FK_PersonCalendar_TimeTypeId] 
	FOREIGN KEY (TimeTypeId)
	REFERENCES [dbo].[TimeType] (TimeTypeId) ON DELETE NO ACTION ON UPDATE NO ACTION;
