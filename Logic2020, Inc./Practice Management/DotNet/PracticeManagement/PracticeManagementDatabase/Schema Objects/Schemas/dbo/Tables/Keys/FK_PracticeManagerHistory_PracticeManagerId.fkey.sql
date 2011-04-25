ALTER TABLE [dbo].[PracticeManagerHistory]  
	ADD  CONSTRAINT [FK_PracticeManagerHistory_PracticeManagerId] FOREIGN KEY([PracticeManagerId])
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE ON UPDATE NO ACTION;
