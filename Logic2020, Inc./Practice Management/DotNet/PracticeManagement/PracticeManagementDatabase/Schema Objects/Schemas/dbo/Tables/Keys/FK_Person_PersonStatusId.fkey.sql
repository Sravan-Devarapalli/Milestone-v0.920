ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_PersonStatusId] FOREIGN KEY ([PersonStatusId]) REFERENCES [dbo].[PersonStatus] ([PersonStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


