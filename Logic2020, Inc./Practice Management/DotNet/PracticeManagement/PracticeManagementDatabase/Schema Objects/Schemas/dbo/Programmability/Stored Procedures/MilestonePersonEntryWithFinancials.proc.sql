CREATE PROCEDURE [dbo].[MilestonePersonEntryWithFinancials]
(
	@Id   INT
)
AS
BEGIN

	DECLARE @IdLocal INT = @Id

    -- Added @MileStoneId to improve performance of FinancialsRetro CTE
	DECLARE @MileStoneId INT 
	SELECT @MileStoneId = m.MilestoneId
	FROM dbo.MilestonePerson AS mp
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId AND mpe.Id = @IdLocal
	INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId


	SELECT 
	CASE COUNT(te.TimeEntryId)
	           WHEN 0
	           THEN  CONVERT(BIT,0)
	           ELSE
	               CONVERT(BIT,1)
	       END AS HasTimeEntries,
	       mp.MilestonePersonId,
		   p.SeniorityId,
           mp.PersonId,
	       mpe.StartDate,
	       mpe.EndDate,
	       mpe.PersonRoleId,
	       mpe.Amount,
	       mpe.HoursPerDay,
	       r.Name AS RoleName,
		   ISNULL((SELECT COUNT(*)
				FROM dbo.v_PersonCalendar AS pcal
				WHERE pcal.DayOff = 1 AND pcal.CompanyDayOff = 0 AND pcal.IsFloatingHoliday = 0
					AND pcal.Date BETWEEN mpe.StartDate AND mpe.EndDate
					AND pcal.PersonId = mp.PersonId ),0) as VacationDays,	
	       ISNULL((SELECT COUNT(*) * mpe.HoursPerDay
				FROM dbo.v_PersonCalendar AS pcal
				WHERE  (
						(pcal.CompanyDayOff = 0 AND pcal.IsFloatingHoliday = 0) 
						OR (pcal.CompanyDayOff = 1 AND pcal.DayOff =  0)
					   )
					AND pcal.Date BETWEEN mpe.StartDate AND mpe.EndDate
					AND pcal.PersonId = mp.PersonId), 0) AS ExpectedHoursWithVacationDays,
		   p.LastName,
		   p.FirstName,
		   mpe.Id,
		   p.IsStrawman AS IsStrawman
	  FROM dbo.MilestonePerson AS mp
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId AND mpe.Id = @IdLocal
	       INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
	       INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
	       LEFT JOIN dbo.PersonRole AS r ON mpe.PersonRoleId = r.PersonRoleId
	       LEFT JOIN dbo.TimeEntries as te on te.MilestonePersonId = mp.MilestonePersonId
		  AND (te.MilestoneDate BETWEEN mpe.StartDate AND  mpe.EndDate)
	  GROUP BY mpe.Id,mp.MilestonePersonId, p.SeniorityId,mp.PersonId,mpe.StartDate,mpe.EndDate,mpe.PersonRoleId,mpe.Amount,
	       mpe.HoursPerDay,r.Name,mpe.Location,
		   p.LastName,
		   p.FirstName,m.ProjectedDeliveryDate,p.IsStrawman

	 ;WITH FinancialsRetro AS 
	(
	SELECT f.EntryId,
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay
	FROM v_FinancialsRetrospective f
	WHERE f.MilestoneId = @MilestoneId AND f.EntryId = @IdLocal  
	)

	SELECT  f.EntryId,
			ISNULL(SUM(f.PersonMilestoneDailyAmount), 0) AS Revenue,
			ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR 
							  ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin

	  FROM FinancialsRetro AS f
	  GROUP BY   f.EntryId

	 SELECT  personid,Date,ActualHours 
	 FROM dbo.[GetPersonTimeoffValuesByMilestoneId](NULL,NULL,NULL,@Id)


END

