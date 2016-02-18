ALTER TABLE [dbo].[DivisionPracticeArea]
	ADD CONSTRAINT [FK_DivisionPracticeArea_Practice] FOREIGN KEY ([PracticeId]) REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


