-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-28-2012
-- Description:	Retrieves the list of projects by the name.
-- Updated by : Sainath.CH
-- Update Date: 03-30-2012
-- =============================================
CREATE PROCEDURE [dbo].[ProjectSearchByName]
(
	@Looked              NVARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;
		

		DECLARE @SearchText NVARCHAR(257)
		SET @SearchText = '%' + ISNULL(RTRIM(LTRIM(@Looked)), '') + '%'

		  SELECT P.Name AS ProjectName,
			   P.ProjectNumber,
			   C.Name AS ClientName
		  FROM dbo.Project AS P
		  INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = P.ProjectStatusId
		  INNER JOIN dbo.Client AS C ON C.ClientId = P.ClientId
		  WHERE  P.ProjectStatusId IN (2,3,4,6) AND P.Name LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	
			/*
			ProjectStatusId	Name
						2	Projected
						3	Active
						4	Completed
						6	Internal

			*/
		   

		  ORDER BY P.Name
END
