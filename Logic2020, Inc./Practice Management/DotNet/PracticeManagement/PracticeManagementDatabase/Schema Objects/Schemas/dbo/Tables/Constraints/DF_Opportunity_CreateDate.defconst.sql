ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [DF_Opportunity_CreateDate] DEFAULT (getdate()) FOR [CreateDate];


