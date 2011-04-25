ALTER TABLE [dbo].[OpportunityTransition]
    ADD CONSTRAINT [FK_OpportunityTransition_Person] FOREIGN KEY ([TargetPersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


