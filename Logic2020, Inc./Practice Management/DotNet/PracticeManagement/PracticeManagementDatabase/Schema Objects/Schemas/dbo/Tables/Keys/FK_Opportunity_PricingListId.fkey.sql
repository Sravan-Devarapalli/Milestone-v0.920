ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_PricingListId] FOREIGN KEY ([PricingListId]) REFERENCES [dbo].[PricingList] ([PricingListId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
