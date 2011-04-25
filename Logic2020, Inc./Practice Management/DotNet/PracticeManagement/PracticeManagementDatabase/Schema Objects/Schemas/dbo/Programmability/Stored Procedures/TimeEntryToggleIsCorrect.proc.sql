-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-19
-- Description:	Toggles is correct
-- =============================================
CREATE PROCEDURE dbo.TimeEntryToggleIsCorrect
	@TimeEntryId INT 
AS
BEGIN
	UPDATE dbo.TimeEntries
	SET IsCorrect = 1 - IsCorrect
	WHERE TimeEntryId = @TimeEntryId
END

