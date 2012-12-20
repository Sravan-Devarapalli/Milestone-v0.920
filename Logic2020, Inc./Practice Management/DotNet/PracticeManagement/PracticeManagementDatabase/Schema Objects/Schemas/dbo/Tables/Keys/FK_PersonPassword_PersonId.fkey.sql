ALTER TABLE [dbo].[PersonPassword]
    ADD CONSTRAINT [FK_PersonPassword_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


