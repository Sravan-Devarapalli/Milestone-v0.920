ALTER TABLE [dbo].[Practice]
    ADD CONSTRAINT [DF_Practice_IsCompanyInternal] DEFAULT ((0)) FOR [IsCompanyInternal];


