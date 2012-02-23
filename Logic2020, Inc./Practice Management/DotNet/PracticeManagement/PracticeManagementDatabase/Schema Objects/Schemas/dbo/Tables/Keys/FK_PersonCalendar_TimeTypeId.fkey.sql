ALTER TABLE [dbo].[PersonCalendar]
	ADD CONSTRAINT [FK_PersonCalendar_TimeTypeId] 
	FOREIGN KEY (TimeTypeId)
	REFERENCES [dbo].[TimeType] (TimeTypeId)
