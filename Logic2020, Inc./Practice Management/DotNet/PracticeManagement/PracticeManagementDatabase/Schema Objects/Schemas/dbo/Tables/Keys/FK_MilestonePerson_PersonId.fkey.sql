ALTER TABLE [dbo].[MilestonePerson]
    ADD CONSTRAINT [FK_MilestonePerson_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


