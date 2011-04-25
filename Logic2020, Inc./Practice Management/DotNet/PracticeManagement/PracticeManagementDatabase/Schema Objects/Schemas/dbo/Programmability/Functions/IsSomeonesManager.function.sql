-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-01-18
-- Description:	Determines if the person is a manager to somebody
-- =============================================
CREATE FUNCTION dbo.IsSomeonesManager 
(
	@PersonId INT 
)
RETURNS BIT 
AS
BEGIN
	DECLARE @CurrentStatus INT 

	-- Check persons current status
	SELECT @CurrentStatus = p.PersonStatusId 
	FROM dbo.Person AS p 
	WHERE p.PersonId = @PersonId
	
	--	If the person is not currently active
	--		it cannot be someones manager, 
	--		so return false
	IF @CurrentStatus != 1
		RETURN 0

	-- Count number of persons who are wokring 
	--	under this manager
	DECLARE @SubCount INT 
	SET @SubCount = 0
	
	SELECT @SubCount = COUNT(*)
	FROM dbo.Person AS p 
	WHERE p.ManagerId = @PersonId

	RETURN CASE
				WHEN @SubCount = 0 THEN 0
				ELSE 1
			END  
END

