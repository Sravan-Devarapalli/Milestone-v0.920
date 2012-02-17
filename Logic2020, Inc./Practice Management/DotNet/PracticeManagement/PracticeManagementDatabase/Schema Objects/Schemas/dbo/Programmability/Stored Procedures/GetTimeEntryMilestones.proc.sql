CREATE PROCEDURE [dbo].[GetTimeEntryMilestones] 
	@PersonId INT,
	@StartDate DATETIME,
	@EndDate DATETIME

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DefaultMilestoneId INT
	

	;WITH ConsMiles AS (
	SELECT 
		cl.[Name] AS 'ClientName',
		cl.ClientId,
		pr.[Name] AS 'ProjectName',
		pr.ProjectId,
		pr.ProjectStatusId,
		pr.ProjectNumber,
		m.Description AS 'MilestoneName',
		m.MilestoneId,
		m.ConsultantsCanAdjust AS 'ConsultantsCanAdjust',
		m.IsChargeable,
		CASE 
			WHEN DATEDIFF(DAY, @StartDate, mpe.StartDate) >= 0 THEN mpe.StartDate
			ELSE @StartDate
		END AS 'PersonStartDate',
		mpe.MilestonePersonId,
		mpe.StartDate,
		CASE
			WHEN DATEDIFF(DAY, mpe.EndDate, @EndDate) >= 0 THEN mpe.EndDate
			ELSE @EndDate
		END AS 'PersonEndDate',
		mpe.EndDate,
		mpe.HoursPerDay,
		mpe.Amount		
	FROM dbo.Milestone AS m
	INNER JOIN dbo.MilestonePerson AS mp ON m.MilestoneId = mp.MilestoneId
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	INNER JOIN dbo.Project AS pr ON m.ProjectId = pr.ProjectId
	INNER JOIN dbo.Client AS cl ON pr.ClientId = cl.ClientId
	WHERE mp.PersonId = @PersonId AND 
		   
	      (mpe.StartDate BETWEEN @StartDate AND @EndDate 
			OR 
			mpe.EndDate BETWEEN @StartDate AND @EndDate
			OR 
			(DATEDIFF(DAY, mpe.StartDate, @StartDate) >= 0 AND DATEDIFF(DAY, @EndDate, mpe.EndDate) >= 0)
			OR
			(DATEDIFF(DAY, @StartDate, mpe.StartDate) >= 0 AND DATEDIFF(DAY, mpe.EndDate, @EndDate) >= 0)
		   )
		   AND( pr.ProjectStatusId  IN (2,3,6)
		   OR EXISTS (SELECT TOP 1 1 FROM v_TimeEntries te
						WHERE te.MilestoneDate BETWEEN @StartDate AND @EndDate
						AND te.PersonId = @PersonId 
						AND te.MilestoneId = m.MilestoneId)
			 )
		    
		   
	)
	SELECT *
	FROM ConsMiles AS cm
	ORDER BY cm.StartDate DESC 
END


