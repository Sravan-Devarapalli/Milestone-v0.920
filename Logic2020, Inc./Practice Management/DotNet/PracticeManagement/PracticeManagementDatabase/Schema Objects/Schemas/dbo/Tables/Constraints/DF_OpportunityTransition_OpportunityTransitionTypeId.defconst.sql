ALTER TABLE [dbo].[OpportunityTransition]
    ADD CONSTRAINT [DF_OpportunityTransition_OpportunityTransitionTypeId] DEFAULT ((1)) FOR [OpportunityTransitionTypeId];


