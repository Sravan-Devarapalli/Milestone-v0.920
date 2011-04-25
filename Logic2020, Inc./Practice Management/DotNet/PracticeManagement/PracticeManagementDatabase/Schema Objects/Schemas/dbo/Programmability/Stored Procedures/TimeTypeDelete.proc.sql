-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Remove existing time type
-- =============================================
CREATE PROCEDURE TimeTypeDelete
	@TimeTypeId INT
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM TimeType WHERE TimeTypeId = @TimeTypeId
END

