ALTER TABLE [dbo].[DefaultCommission]
    ADD CONSTRAINT [FK_DefaultCommission_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


