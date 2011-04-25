-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-02
-- Description:	Removes TE
-- =============================================
CREATE PROCEDURE dbo.TimeEntryRemove
	@TimeEntryId INT 
AS
BEGIN
	SET NOCOUNT ON;

	DELETE 
	FROM TimeEntries 
	WHERE TimeEntryId = @TimeEntryId
END

