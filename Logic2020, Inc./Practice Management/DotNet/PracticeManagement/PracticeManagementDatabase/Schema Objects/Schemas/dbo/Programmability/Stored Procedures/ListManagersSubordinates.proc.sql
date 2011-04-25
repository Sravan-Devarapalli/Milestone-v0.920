-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-19
-- Description:	Lists manager's subordinates
-- =============================================
CREATE PROCEDURE dbo.ListManagersSubordinates 
	@PersonId INT AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT p.FirstName, p.LastName, p.PersonId, p.IsDefaultManager
	FROM dbo.Person AS p 
	WHERE p.ManagerId = @PersonId
END

