ALTER TABLE [dbo].[PracticeLeadership]
	ADD CONSTRAINT [FK_PracticeLeadership_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[PersonDivision]([DivisionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


