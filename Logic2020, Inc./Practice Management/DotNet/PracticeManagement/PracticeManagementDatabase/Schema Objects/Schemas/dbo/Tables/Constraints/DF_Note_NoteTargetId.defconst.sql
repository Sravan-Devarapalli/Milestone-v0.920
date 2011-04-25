ALTER TABLE [dbo].[Note]
    ADD CONSTRAINT [DF_Note_NoteTargetId] DEFAULT ((1)) FOR [NoteTargetId];


