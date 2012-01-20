ALTER TABLE [dbo].[PersonTimeEntryRecursiveSelection]
	ADD CONSTRAINT [FK_PersonTimeEntryRecursiveSelection_TimeEntrySectionId] 
	FOREIGN KEY (TimeEntrySectionId)
	REFERENCES TimeEntrySection (Id) ON DELETE NO ACTION ON UPDATE NO ACTION;
