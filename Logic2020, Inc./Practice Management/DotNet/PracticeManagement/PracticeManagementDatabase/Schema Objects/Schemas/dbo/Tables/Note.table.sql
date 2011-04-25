CREATE TABLE [dbo].[Note] (
    [NoteId]       INT            IDENTITY (1, 1) NOT NULL,
    [TargetId]     INT            NOT NULL,
    [PersonId]     INT            NOT NULL,
    [CreateDate]   DATETIME       NOT NULL,
    [NoteText]     NVARCHAR (MAX) NOT NULL,
    [NoteTargetId] INT            NOT NULL
);


