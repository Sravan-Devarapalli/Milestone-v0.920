ALTER TABLE [dbo].[ChargeCode]
	ADD CONSTRAINT [FK_ChargeCode_TimeEntrySectionId] 
	FOREIGN KEY (TimeEntrySectionId)
	REFERENCES TimeEntrySection (Id) ON DELETE NO ACTION ON UPDATE NO ACTION;
