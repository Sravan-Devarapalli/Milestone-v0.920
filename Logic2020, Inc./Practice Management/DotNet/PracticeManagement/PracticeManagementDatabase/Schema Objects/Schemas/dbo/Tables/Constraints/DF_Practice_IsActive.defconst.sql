ALTER TABLE [dbo].[Practice]
    ADD CONSTRAINT [DF_Practice_IsActive] DEFAULT ((1)) FOR [IsActive];


