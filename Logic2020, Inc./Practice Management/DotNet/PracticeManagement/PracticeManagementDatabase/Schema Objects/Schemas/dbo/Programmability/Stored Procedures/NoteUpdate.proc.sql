CREATE PROCEDURE [dbo].[NoteUpdate]
(
	@NoteId        INT ,
	@NoteText      NVARCHAR(MAX)
)
AS
	SET NOCOUNT ON
	UPDATE dbo.Note
	SET    NoteText = @NoteText
	WHERE  NoteId = @NoteId

