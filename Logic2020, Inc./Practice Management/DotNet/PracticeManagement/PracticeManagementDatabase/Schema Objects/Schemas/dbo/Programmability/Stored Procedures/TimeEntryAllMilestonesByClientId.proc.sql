CREATE  PROCEDURE [dbo].[TimeEntryAllMilestonesByClientId] 
@ClientId INT = NULL,
@PersonId INT = NULL,
@ShowAll BIT = 1
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
				LEFT JOIN dbo.Commission AS C ON C.ProjectId = proj.ProjectId AND C.CommissionType = 1 --1 is SalesCommission
        WHERE   (proj.ClientId =  @ClientId  OR @ClientId IS NULL)
				AND (
						@PersonId IS NULL
						OR proj.ProjectManagerId = @PersonId
						OR proj.DirectorId = @PersonId
						OR C.PersonId = @PersonId
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
        JOIN Milestone M ON M.ProjectId = P.ProjectId
		LEFT JOIN dbo.Commission AS C ON C.ProjectId = P.ProjectId AND C.CommissionType = 1 --1 is SalesCommission
        WHERE (@ClientId IS NULL OR P.ClientId = @ClientId)
				AND (
						@PersonId IS NULL
						OR P.ProjectManagerId = @PersonId
						OR P.DirectorId = @PersonId
						OR C.PersonId = @PersonId
					)
              AND (@ShowAll = 1 OR (@ShowAll = 0 AND (P.ProjectStatusId = 3 or P.ProjectStatusId = 6) ) )
        
END
