CREATE PROCEDURE [dbo].[PersonMilestoneWithFinancials] 
(
	@PersonId         INT
)
AS
BEGIN

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	SELECT  p.ProjectId,
			p.Name AS 'ProjectName',
			p.ProjectStatusId,
			p.ProjectNumber,
			s.Name AS 'ProjectStatus',
			m.MilestoneId,
			m.Description AS 'MilestoneName',
			mp.MilestonePersonId,
			mpe.StartDate,
			mpe.EndDate,
			r.Name AS 'RoleName',
			f.Revenue,
			f.GrossMargin,
			pa.[FileName]
	FROM    dbo.Project AS p
			INNER JOIN dbo.ProjectStatus AS s ON p.ProjectStatusId = s.ProjectStatusId
			INNER JOIN dbo.Milestone AS m ON p.ProjectId = m.ProjectId
			INNER JOIN dbo.MilestonePerson AS mp ON m.MilestoneId = mp.MilestoneId
			INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
			LEFT JOIN dbo.PersonRole AS r ON mpe.PersonRoleId = r.PersonRoleId
			LEFT JOIN dbo.ProjectAttachment AS pa ON p.ProjectId = pa.ProjectId
			INNER JOIN v_FinancialsForMilestones AS f ON m.MilestoneId = f.MilestoneId
														 AND mp.PersonId = f.PersonId
	WHERE   f.PersonId = @PersonId AND P.ProjectId <> @DefaultProjectId
	ORDER BY mpe.StartDate
END


