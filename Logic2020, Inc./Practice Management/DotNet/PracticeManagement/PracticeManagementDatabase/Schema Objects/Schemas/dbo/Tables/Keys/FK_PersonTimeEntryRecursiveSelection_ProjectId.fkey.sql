﻿ALTER TABLE [dbo].[PersonTimeEntryRecursiveSelection]
	ADD CONSTRAINT [FK_PersonTimeEntryRecursiveSelection_ProjectId] 
	FOREIGN KEY ([ProjectId])
	REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
