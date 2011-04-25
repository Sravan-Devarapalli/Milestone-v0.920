-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-19
-- Description:	Toggles is correct
-- =============================================
CREATE PROCEDURE dbo.TimeEntryToggleIsReviewed
	@TimeEntryId INT 
AS
BEGIN
	UPDATE dbo.TimeEntries
	SET IsReviewed =
	CASE
		WHEN IsReviewed IS NULL THEN 1
		WHEN IsReviewed = 1 THEN 0
		ELSE NULL
	END 
	WHERE TimeEntryId = @TimeEntryId
END

