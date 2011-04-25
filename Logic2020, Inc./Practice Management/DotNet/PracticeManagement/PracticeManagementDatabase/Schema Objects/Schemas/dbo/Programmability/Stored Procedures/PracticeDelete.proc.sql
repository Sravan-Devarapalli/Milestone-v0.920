-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-26
-- Description:	Practices routines
-- =============================================
CREATE PROCEDURE dbo.PracticeDelete 
	@PracticeId INT	
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.PracticeStatusHistory
	WHERE PracticeId = @PracticeId

	DELETE FROM dbo.PracticeManagerHistory
	WHERE PracticeId = @PracticeId

	DELETE FROM Practice 
	 WHERE PracticeId = @PracticeId
 
 END

