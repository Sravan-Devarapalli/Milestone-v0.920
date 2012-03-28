-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-28-2012
-- =============================================
CREATE  PROCEDURE [dbo].[GetProjectsByClientId] 
(
	@ClientId INT
)
AS
BEGIN
	SET NOCOUNT ON;
	
    SELECT  P.ProjectId,
			P.Name AS 'ProjectName',
			P.ProjectNumber,
			PS.Name AS ProjectStatusName
    FROM dbo.Project P
	INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = P.ProjectStatusId
    WHERE P.ClientId = @ClientId AND P.ProjectStatusId IN (2,3,4,6)
	
	/*
	ProjectStatusId	Name
				2	Projected
				3	Active
				4	Completed
				6	Internal

	*/
            
        
END
