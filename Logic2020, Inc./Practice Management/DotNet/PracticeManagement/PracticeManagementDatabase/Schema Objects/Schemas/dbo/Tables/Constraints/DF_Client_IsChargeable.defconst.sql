ALTER TABLE [dbo].[Client]
    ADD CONSTRAINT [DF_Client_IsChargeable] DEFAULT ((1)) FOR [IsChargeable];


