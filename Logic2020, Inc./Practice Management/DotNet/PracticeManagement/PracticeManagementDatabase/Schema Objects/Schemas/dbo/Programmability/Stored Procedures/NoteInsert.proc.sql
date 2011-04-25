CREATE PROCEDURE [dbo].[NoteInsert]
(
	@TargetId		INT,
	@NoteTargetId	INT,
	@PersonId		INT,
	@NoteText		NVARCHAR(MAX),
	@NoteId         INT OUT 
)
AS
	SET NOCOUNT ON

	INSERT INTO dbo.Note
	            (
	              TargetId, 
	              NoteTargetId, 
	              PersonId, 
	              NoteText
	             )
	VALUES       (
				  @TargetId, 
				  @NoteTargetId, 
				  @PersonId, 
				  @NoteText
				  )
    SET @NoteId = SCOPE_IDENTITY()
    SELECT @NoteId
