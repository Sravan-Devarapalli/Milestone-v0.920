CREATE PROCEDURE [dbo].[PersonMilestoneWithFinancials] 
(
	@PersonId         INT
)
AS
BEGIN

	DECLARE @DefaultProjectId INT,
			@PersonIdLocal INT

	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	SELECT @PersonIdLocal = @PersonId

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.MilestoneId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
		   	+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount,
		   f.EntryId
	FROM v_FinancialsRetrospective f
	WHERE f.PersonId = @PersonIdLocal  AND f.ProjectId != @DefaultProjectId
	)

	SELECT  p.ProjectId,
			p.Name AS 'ProjectName',
			p.ProjectStatusId,
			p.ProjectNumber,
			s.Name AS 'ProjectStatus',
			m.MilestoneId,
			m.Description AS 'MilestoneName',
			f.EntryId,
			mp.MilestonePersonId,
			mpe.StartDate,
			mpe.EndDate,
			r.Name AS 'RoleName',
			CASE WHEN A.ProjectId IS NOT NULL THEN 1 
					ELSE 0 END AS HasAttachments,
	       ISNULL(SUM(f.PersonMilestoneDailyAmount), 0) AS Revenue,
		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin
	  FROM FinancialsRetro AS f
	  INNER JOIN dbo.Project AS p ON p.ProjectId = F.ProjectId
	  INNER JOIN dbo.ProjectStatus AS s ON p.ProjectStatusId = s.ProjectStatusId
	  INNER JOIN dbo.Milestone AS m ON p.ProjectId = m.ProjectId
	  INNER JOIN dbo.MilestonePerson AS mp ON m.MilestoneId = mp.MilestoneId
	  INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId AND mpe.Id = f.EntryId
	  LEFT JOIN dbo.PersonRole AS r ON mpe.PersonRoleId = r.PersonRoleId
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
	  WHERE f.PersonId = @PersonIdLocal AND P.ProjectId <> @DefaultProjectId
	  GROUP BY f.EntryId,p.ProjectId,p.Name,p.ProjectStatusId,p.ProjectNumber,m.MilestoneId,m.Description,mpe.StartDate,
			mpe.EndDate,r.Name,A.ProjectId,s.Name,mp.MilestonePersonId
	  ORDER BY mpe.StartDate
END

