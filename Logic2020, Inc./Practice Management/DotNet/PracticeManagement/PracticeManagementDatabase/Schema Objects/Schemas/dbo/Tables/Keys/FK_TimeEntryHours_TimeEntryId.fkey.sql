ALTER TABLE [dbo].[TimeEntryHours]
	ADD CONSTRAINT [FK_TimeEntryHours_TimeEntryId] 
	FOREIGN KEY (TimeEntryId)
	REFERENCES TimeEntry (TimeEntryId)  ON DELETE NO ACTION ON UPDATE NO ACTION;

