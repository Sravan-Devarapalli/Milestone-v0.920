ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_PriorityId] FOREIGN KEY (PriorityId) REFERENCES dbo.OpportunityPriorities (Id) ON DELETE NO ACTION ON UPDATE NO ACTION;

