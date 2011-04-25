-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-23
-- =============================================
CREATE PROCEDURE NotesClone
	@OldTargetId INT, 
	@NewTargetId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Note]
           ([TargetId]
           ,[PersonId]
           ,[CreateDate]
           ,[NoteText]
           ,[NoteTargetId])
     select @NewTargetId, PersonId, CreateDate, NoteText, NoteTargetId
	 from Note
	 where TargetId = @OldTargetId
END

