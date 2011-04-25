-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-01-18
-- Description:	Sets new manager
-- =============================================
CREATE PROCEDURE dbo.SetNewManager 
	@OldManagerId INT, 
	@NewManagerId INT 
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Person
	SET ManagerId = @NewManagerId
	WHERE ManagerId = @OldManagerId
END

