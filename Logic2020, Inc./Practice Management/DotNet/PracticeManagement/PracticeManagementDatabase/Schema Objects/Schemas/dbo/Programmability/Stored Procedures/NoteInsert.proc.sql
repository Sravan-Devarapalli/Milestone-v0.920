CREATE PROCEDURE [dbo].[NoteInsert]
(
	@TargetId		INT,
	@NoteTargetId	INT,
	@PersonId		INT,
	@NoteText		NVARCHAR(MAX),
	@NoteId         INT OUT 
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @UserLogin NVARCHAR(255)

	SELECT @UserLogin = Alias FROM dbo.Person WHERE PersonId = @PersonId

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin 

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

	-- End logging session
	EXEC dbo.SessionLogUnprepare
    
	SELECT @NoteId

END
