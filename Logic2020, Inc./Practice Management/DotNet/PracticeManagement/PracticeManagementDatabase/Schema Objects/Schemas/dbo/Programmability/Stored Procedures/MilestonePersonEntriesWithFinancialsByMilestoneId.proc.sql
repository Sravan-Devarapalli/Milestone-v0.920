CREATE PROCEDURE [dbo].[MilestonePersonEntriesWithFinancialsByMilestoneId]
	@MilestoneId      INT
AS
BEGIN
	
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
				WHERE pcal.DayOff = 1 AND pcal.CompanyDayOff = 0 
					AND pcal.Date BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, m.[ProjectedDeliveryDate])
					AND pcal.PersonId = mp.PersonId ),0) as VacationDays,	
	       ISNULL((SELECT COUNT(*) * mpe.HoursPerDay
	                 FROM dbo.PersonCalendarAuto AS cal
	                WHERE cal.Date BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, m.[ProjectedDeliveryDate])
	                  AND cal.PersonId = mp.PersonId
	                  AND cal.DayOff = 0), 0) AS ExpectedHours,
		   mpe.Location,
		   p.LastName,
		   p.FirstName
		  
	  FROM dbo.MilestonePerson AS mp
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	       INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
	       INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
	       LEFT JOIN dbo.PersonRole AS r ON mpe.PersonRoleId = r.PersonRoleId
	       LEFT JOIN dbo.TimeEntries as te on te.MilestonePersonId = mp.MilestonePersonId
		  AND (te.MilestoneDate BETWEEN mpe.StartDate AND  mpe.EndDate)
	  WHERE mp.MilestoneId = @MilestoneId
	  GROUP BY mp.MilestonePersonId, p.SeniorityId,mp.PersonId,mpe.StartDate,mpe.EndDate,mpe.PersonRoleId,mpe.Amount,
	       mpe.HoursPerDay,r.Name,mpe.Location,
		   p.LastName,
		   p.FirstName,m.ProjectedDeliveryDate
	 


	 ;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.MilestoneId,
		   f.EntryStartDate,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   ISNULL(c.FractionOfMargin,0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
		   	+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	LEFT JOIN (
					SELECT ProjectId,SUM(FractionOfMargin) FractionOfMargin
					FROM dbo.Commission 
					WHERE CommissionType = 1
					GROUP BY ProjectId
		) C ON C.ProjectId = f.ProjectId
	WHERE f.MilestoneId = @MilestoneId  
	)

	SELECT f.ProjectId,
			f.MilestoneId,
		   f.PersonId,
		   f.EntryStartDate StartDate,   
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       ISNULL(SUM(f.PersonMilestoneDailyAmount), 0) AS Revenue,

	       ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount), 0) AS RevenueNet,

		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
					 THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)), 0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	       (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR  ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
	           0.0 AS 'actualhours',
	           0.0 AS 'forecastedhours',
           ISNULL(SUM(vac.VacationHours), 0) AS 'VacationHours'
		   
	  FROM FinancialsRetro AS f
	  LEFT JOIN dbo.v_MilestonePersonVacations AS vac ON f.MilestoneId = vac.MilestoneId AND f.PersonId = vac.PersonId
	  GROUP BY   f.ProjectId, f.MilestoneId, f.PersonId,f.EntryStartDate
END
