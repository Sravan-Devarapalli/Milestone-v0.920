ALTER TABLE [dbo].[Milestone]
    ADD CONSTRAINT [DF_Milestone_IsChargeable] DEFAULT ((1)) FOR [IsChargeable];


