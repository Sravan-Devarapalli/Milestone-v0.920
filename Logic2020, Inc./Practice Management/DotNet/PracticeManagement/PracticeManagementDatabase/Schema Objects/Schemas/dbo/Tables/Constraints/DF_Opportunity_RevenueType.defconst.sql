ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [DF_Opportunity_RevenueType] DEFAULT ((3)) FOR [RevenueType];


