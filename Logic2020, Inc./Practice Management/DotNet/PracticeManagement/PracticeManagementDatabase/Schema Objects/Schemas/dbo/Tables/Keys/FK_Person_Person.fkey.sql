ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_Person] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


