ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_RevenueType] FOREIGN KEY ([RevenueType]) REFERENCES [dbo].[RevenueType] ([RevenueTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


