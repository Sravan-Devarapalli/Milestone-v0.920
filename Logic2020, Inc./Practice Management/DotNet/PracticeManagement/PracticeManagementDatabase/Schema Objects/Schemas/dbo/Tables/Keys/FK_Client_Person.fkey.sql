ALTER TABLE [dbo].[Client]
    ADD CONSTRAINT [FK_Client_Person] FOREIGN KEY ([DefaultSalespersonID]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


