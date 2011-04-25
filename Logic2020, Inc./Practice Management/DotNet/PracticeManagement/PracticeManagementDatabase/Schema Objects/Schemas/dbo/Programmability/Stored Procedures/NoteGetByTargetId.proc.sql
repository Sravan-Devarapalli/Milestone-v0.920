-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-06-2008
-- =============================================
CREATE PROCEDURE dbo.NoteGetByTargetId
(
	@TargetId   INT,
	@NoteTargetId INT
)
AS
	SET NOCOUNT ON

	SELECT n.NoteId, n.TargetId, n.PersonId, n.CreateDate, n.NoteText, n.LastName, n.FirstName
	  FROM dbo.v_Notes as n
	 WHERE n.TargetId = @TargetId AND n.NoteTargetId = @NoteTargetId	
	ORDER BY n.CreateDate DESC

