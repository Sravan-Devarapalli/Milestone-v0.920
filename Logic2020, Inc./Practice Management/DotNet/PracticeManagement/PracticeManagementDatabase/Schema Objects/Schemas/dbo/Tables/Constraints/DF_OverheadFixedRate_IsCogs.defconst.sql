ALTER TABLE [dbo].[OverheadFixedRate]
    ADD CONSTRAINT [DF_OverheadFixedRate_IsCogs] DEFAULT ((1)) FOR [IsCogs];


