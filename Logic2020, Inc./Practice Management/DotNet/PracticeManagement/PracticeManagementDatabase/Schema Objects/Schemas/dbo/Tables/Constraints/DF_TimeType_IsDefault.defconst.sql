ALTER TABLE [dbo].[TimeType]
    ADD CONSTRAINT [DF_TimeType_IsDefault] DEFAULT ((0)) FOR [IsDefault];


