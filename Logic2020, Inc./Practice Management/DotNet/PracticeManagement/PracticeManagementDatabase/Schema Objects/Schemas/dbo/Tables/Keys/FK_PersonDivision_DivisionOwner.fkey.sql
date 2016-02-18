ALTER TABLE [dbo].[PersonDivision]
	ADD CONSTRAINT [FK_PersonDivision_DivisionOwner] FOREIGN KEY ([DivisionOwnerId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


