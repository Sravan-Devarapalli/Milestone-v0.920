ALTER TABLE [dbo].[Permission]
    ADD CONSTRAINT [FK_Permission_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


