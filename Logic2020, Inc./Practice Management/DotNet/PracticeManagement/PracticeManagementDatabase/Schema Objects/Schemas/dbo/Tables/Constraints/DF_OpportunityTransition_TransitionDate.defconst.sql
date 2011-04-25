ALTER TABLE [dbo].[OpportunityTransition]
    ADD CONSTRAINT [DF_OpportunityTransition_TransitionDate] DEFAULT (getdate()) FOR [TransitionDate];


