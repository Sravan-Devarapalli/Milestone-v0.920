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
		   P.Name,
		   C.Name AS ClientName,
		   PG.Name AS GroupName,
		   PS.Name AS ProjectStatusName	      
	  FROM dbo.Project AS P
	  INNER JOIN dbo.Client AS C ON C.ClientId = P.ClientId
	  INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = P.GroupId
	  INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = P.ProjectStatusId
	  WHERE P.ProjectNumber = @ProjectNumber     

END
