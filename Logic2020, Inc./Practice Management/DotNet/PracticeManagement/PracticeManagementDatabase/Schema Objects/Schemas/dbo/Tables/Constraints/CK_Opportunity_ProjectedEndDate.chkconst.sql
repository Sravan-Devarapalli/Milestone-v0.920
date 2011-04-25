ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [CK_Opportunity_ProjectedEndDate] CHECK ([ProjectedEndDate]>=[ProjectedStartDate] OR [ProjectedEndDate] IS NULL);


