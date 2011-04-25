ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [FK_Pay_PracticeId] FOREIGN KEY ([PracticeId]) 
	REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
