ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [DF_Opportunity_LastUpdated] DEFAULT (getdate()) FOR [LastUpdated];


