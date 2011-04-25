-- =============================================
-- Description:	Retrives a Project Id
-- =============================================
-- Arguments:
--     @ProjectNumber - a Number of the project to be retrieved.
-- =============================================
CREATE PROCEDURE dbo.ProjectGetByNumber
(
	@ProjectNumber nvarchar(24)
)
AS
	SET NOCOUNT ON

	SELECT p.ProjectId	      
	  FROM dbo.v_Project AS p
	 WHERE p.ProjectNumber = @ProjectNumber     

