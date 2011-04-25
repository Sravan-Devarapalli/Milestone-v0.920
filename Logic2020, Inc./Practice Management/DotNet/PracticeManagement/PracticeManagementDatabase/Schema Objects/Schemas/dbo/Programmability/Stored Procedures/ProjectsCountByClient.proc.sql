-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-22
-- Description:	Counts number of projects for given client
-- =============================================
CREATE PROCEDURE dbo.ProjectsCountByClient
	@ClientId INT
AS
BEGIN
		SELECT COUNT(*)
		  FROM dbo.v_Project AS p
		 WHERE p.ClientId = @ClientId
END

