ALTER TABLE [dbo].[TimeEntries]
    ADD CONSTRAINT [DF_TimeEntries_IsChargeable] DEFAULT ((1)) FOR [IsChargeable];


