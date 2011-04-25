ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_Person] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


