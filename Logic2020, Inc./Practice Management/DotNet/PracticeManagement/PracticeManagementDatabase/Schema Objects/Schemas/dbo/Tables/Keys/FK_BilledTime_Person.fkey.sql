ALTER TABLE [dbo].[BilledTime]
    ADD CONSTRAINT [FK_BilledTime_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


