ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_DivisionId] 
	FOREIGN KEY ([DivisionId]) 
	REFERENCES [dbo].[PersonDivision] ([DivisionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
