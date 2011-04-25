ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_OpportunityStatusId] FOREIGN KEY ([OpportunityStatusId]) REFERENCES [dbo].[OpportunityStatus] ([OpportunityStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


