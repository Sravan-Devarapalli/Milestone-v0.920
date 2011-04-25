ALTER TABLE [dbo].[MilestonePersonEntry]
    ADD CONSTRAINT [DF_MilestonePersonEntry_HoursPerDay] DEFAULT ((0)) FOR [HoursPerDay];


