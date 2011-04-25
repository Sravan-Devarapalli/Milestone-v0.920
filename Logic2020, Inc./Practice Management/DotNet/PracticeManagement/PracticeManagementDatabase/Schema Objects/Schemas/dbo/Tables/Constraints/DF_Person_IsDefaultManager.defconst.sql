ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [DF_Person_IsDefaultManager] DEFAULT ((0)) FOR [IsDefaultManager];


