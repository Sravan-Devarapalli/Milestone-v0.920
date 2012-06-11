ALTER TABLE [dbo].[Client]
    ADD CONSTRAINT [DF_Client_IsNoteRequired] DEFAULT ((1)) FOR [IsNoteRequired];


