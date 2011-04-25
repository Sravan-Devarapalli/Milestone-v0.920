ALTER TABLE [dbo].[MilestonePersonEntry]
    ADD CONSTRAINT [CK_MilestonePersonEntry_HoursPerDay] CHECK ([HoursPerDay]>=(0));


