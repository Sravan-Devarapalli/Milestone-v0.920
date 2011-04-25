-- =============================================
-- Author:		Nikita G.
-- Create date: 2009-12-25
-- Description:	Retrieves all persons that have entered TE at least once
-- =============================================
CREATE  PROCEDURE [dbo].[TimeEntryAllMilestones] 
@ClientId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
        SELECT  DISTINCT m.MilestoneId,
                m.Description AS 'MilestoneName',
                proj.ProjectId,
                proj.[Name] AS 'ProjectName',
				proj.ProjectNumber,
				proj.ClientId
        FROM    TimeEntries AS te
                INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = te.MilestonePersonId
                INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
                INNER JOIN dbo.Milestone AS m ON m.MilestoneId = mp.MilestoneId
                INNER JOIN dbo.Project AS proj ON proj.ProjectId = m.ProjectId
        WHERE   proj.ClientId =  @ClientId  OR @ClientId IS NULL
              
		UNION
        SELECT M.MilestoneId,
				M.Description AS 'MilestoneName',
				P.ProjectId,
				P.Name AS 'ProjectName',
				P.ProjectNumber,
				P.ClientId
        FROM dbo.Project P
        JOIN Milestone M ON M.ProjectId = P.ProjectId
        WHERE (P.ProjectStatusId = 3 AND P.ClientId =  @ClientId ) --Active 
              OR (P.ProjectStatusId = 3 AND  @ClientId IS NULL)
        
END

