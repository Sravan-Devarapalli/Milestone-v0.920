ALTER TABLE [dbo].[OpportunityTransition]
    ADD CONSTRAINT [FK_OpportunityTransition_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE ON UPDATE NO ACTION;


