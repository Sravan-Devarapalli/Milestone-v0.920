ALTER TABLE [dbo].[DivisionPracticeArea]
	ADD CONSTRAINT [FK_DivisionPracticeArea_PersonDivision] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[PersonDivision] ([DivisionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


