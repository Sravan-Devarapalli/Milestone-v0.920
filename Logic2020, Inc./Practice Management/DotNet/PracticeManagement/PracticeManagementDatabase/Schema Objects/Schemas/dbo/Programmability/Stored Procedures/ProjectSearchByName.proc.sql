-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-28-2012
-- Description:	Retrieves the list of projects by the name.
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
			   P.ProjectNumber
		  FROM dbo.Project AS P
		  WHERE P.Name LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
		  ORDER BY P.Name
END
