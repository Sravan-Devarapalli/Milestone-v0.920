CREATE PROCEDURE dbo.NoteDelete
(
	@NoteId   INT
)
AS
BEGIN
	SET NOCOUNT ON
	IF EXISTS(  
				SELECT TOP 1 1 
				FROM dbo.Note
				WHERE NoteId = @NoteId
			 )
	 BEGIN
		DELETE FROM dbo.Note
		WHERE NoteId = @NoteId
	 END
END
