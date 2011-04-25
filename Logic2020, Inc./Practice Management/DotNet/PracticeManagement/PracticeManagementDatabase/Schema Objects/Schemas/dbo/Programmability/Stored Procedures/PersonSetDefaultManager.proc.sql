-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-01-21
-- Description:	Sets default manager
-- =============================================
CREATE PROCEDURE dbo.PersonSetDefaultManager
	@DefaultManagerId INT 
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Person
	SET IsDefaultManager = 0
	WHERE IsDefaultManager = 1
		
	UPDATE dbo.Person
	SET IsDefaultManager = 1
	WHERE PersonId = @DefaultManagerId
END

