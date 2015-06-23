-- =============================================
-- Updated by:	Sainath C
-- Update date:	06-05-2012
-- =============================================
CREATE PROCEDURE [dbo].[TimeEntryAllMilestonesByClientId] 
(
@ClientId INT = NULL,
@PersonId INT = NULL,
@ShowAll BIT = 1
)
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
				INNER JOIN dbo.ProjectAccess AS projManagers ON proj.ProjectId = projManagers.ProjectId
        WHERE   (proj.ClientId =  @ClientId  OR @ClientId IS NULL)
				AND (
						@PersonId IS NULL
						OR projManagers.ProjectAccessId = @PersonId
						OR proj.ProjectManagerId = @PersonId
						OR proj.ExecutiveInChargeId = @PersonId
						OR proj.SalesPersonId = @PersonId
					)
				AND (@ShowAll = 1 OR (@ShowAll = 0 AND (proj.ProjectStatusId = 3 or proj.ProjectStatusId = 6) ) )
		UNION
        SELECT M.MilestoneId,
				M.Description AS 'MilestoneName',
				P.ProjectId,
				P.Name AS 'ProjectName',
				P.ProjectNumber,
				P.ClientId
        FROM dbo.Project P
		INNER JOIN dbo.ProjectAccess AS projManagers ON P.ProjectId = projManagers.ProjectId
        INNER JOIN Milestone M ON M.ProjectId = P.ProjectId
        WHERE (@ClientId IS NULL OR P.ClientId = @ClientId)
				AND (
						@PersonId IS NULL
						OR projManagers.ProjectAccessId = @PersonId
						OR p.ProjectManagerId = @PersonId
						OR P.ExecutiveInChargeId = @PersonId
						OR P.SalesPersonId = @PersonId
					)
              AND (@ShowAll = 1 OR (@ShowAll = 0 AND (P.ProjectStatusId = 3 or P.ProjectStatusId = 6) ) )
        
END

