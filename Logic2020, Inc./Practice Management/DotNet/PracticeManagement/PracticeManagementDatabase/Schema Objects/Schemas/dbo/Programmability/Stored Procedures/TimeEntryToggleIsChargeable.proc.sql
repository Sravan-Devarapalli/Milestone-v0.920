-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-19
-- Description:	Toggles is correct
-- =============================================
CREATE PROCEDURE dbo.TimeEntryToggleIsChargeable
	@TimeEntryId INT 
AS
BEGIN
	UPDATE dbo.TimeEntries
	SET IsChargeable = 1 - IsChargeable
	WHERE TimeEntryId = @TimeEntryId
END

