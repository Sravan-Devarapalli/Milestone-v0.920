ALTER TABLE [dbo].[PracticeManagerHistory]
	ADD  CONSTRAINT [FK_PracticeManagerHistory_PracticeId] FOREIGN KEY([PracticeId])
	REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE CASCADE ON UPDATE NO ACTION;
	

