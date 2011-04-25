ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [DF_Project_IsChargeable] DEFAULT ((1)) FOR [IsChargeable];


