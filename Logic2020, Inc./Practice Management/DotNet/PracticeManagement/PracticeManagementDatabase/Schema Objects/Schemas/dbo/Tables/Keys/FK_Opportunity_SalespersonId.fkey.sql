ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_SalespersonId] FOREIGN KEY ([SalespersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE SET NULL ON UPDATE NO ACTION;


