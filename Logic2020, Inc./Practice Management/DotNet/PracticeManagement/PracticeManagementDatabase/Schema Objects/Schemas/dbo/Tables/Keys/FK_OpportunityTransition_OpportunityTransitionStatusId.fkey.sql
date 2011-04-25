ALTER TABLE [dbo].[OpportunityTransition]
    ADD CONSTRAINT [FK_OpportunityTransition_OpportunityTransitionStatusId] FOREIGN KEY ([OpportunityTransitionStatusId]) REFERENCES [dbo].[OpportunityTransitionStatus] ([OpportunityTransitionStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


