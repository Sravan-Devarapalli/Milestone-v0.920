-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-28-2012
-- =============================================
CREATE PROCEDURE dbo.ProjectShortGetByNumber
(
	@ProjectNumber NVARCHAR(12)
)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT P.ProjectId,
		   P.StartDate,
		   P.EndDate,
		   P.Name	      
	  FROM dbo.Project AS P
	 WHERE P.ProjectNumber = @ProjectNumber     

END
