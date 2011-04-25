ALTER TABLE [dbo].[Commission]
    ADD CONSTRAINT [FK_Commission_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


