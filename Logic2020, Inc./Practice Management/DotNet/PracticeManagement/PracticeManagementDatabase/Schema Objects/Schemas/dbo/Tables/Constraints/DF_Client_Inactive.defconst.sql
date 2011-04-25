ALTER TABLE [dbo].[Client]
    ADD CONSTRAINT [DF_Client_Inactive] DEFAULT ((0)) FOR [Inactive];


