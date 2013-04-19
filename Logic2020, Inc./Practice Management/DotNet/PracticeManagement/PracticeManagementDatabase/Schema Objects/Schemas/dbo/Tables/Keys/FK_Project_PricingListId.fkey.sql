	ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_PricingListId] FOREIGN KEY ([PricingListId]) REFERENCES [dbo].[PricingList] ([PricingListId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


