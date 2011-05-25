CREATE VIEW [dbo].[v_FinancialsRetrospective]
AS
	SELECT r.ProjectId,
		   r.MilestoneId,
	       r.PracticeManagerId,
		   r.Date,
		   r.IsHourlyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
	           THEN r.MilestoneDailyAmount
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / s.HoursPerDay, r.MilestoneDailyAmount)
	       END AS MilestoneDailyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
	           THEN ISNULL(m.Amount*m.HoursPerDay, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / s.HoursPerDay, r.MilestoneDailyAmount)
	       END AS PersonMilestoneDailyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
	           THEN r.MilestoneDailyAmount * r.Discount / 100
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay * r.Discount / (s.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
	       END AS DiscountDailyAmount,
		   CASE
	           WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
	           THEN ISNULL(m.Amount * m.HoursPerDay * r.Discount / 100, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay * r.Discount / (s.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
	       END AS PersonDiscountDailyAmount,
	       r.Discount,
		   s.HoursPerDay,
		   m.PersonId,
	       m.EntryStartDate,
	       m.HoursPerDay AS PersonHoursPerDay,
		   CASE
	           WHEN r.IsHourlyAmount = 1
	           THEN m.Amount
	           WHEN r.IsHourlyAmount = 0 AND s.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount / s.HoursPerDay
		   END AS BillRate,
		   CASE
	           WHEN r.IsHourlyAmount = 1
	           THEN m.Amount * r.Discount / 100
	           WHEN r.IsHourlyAmount = 0 AND s.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount * r.Discount / (s.HoursPerDay * 100)
		   END AS DiscountRate,
		   p.Timescale,
		   --p.HourlyRate AS PayRate, -- this it how it was before Timescale (4, Hourly/POR)
		   CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * m.Amount
			ELSE p.HourlyRate
		   END AS PayRate, 	-- new payrate that takes into account that % unit is used in the Amount instead of $ unit
	       CASE p.BonusHoursToCollect
	           WHEN 0 THEN 0
	           ELSE p.BonusAmount / (CASE WHEN p.IsYearBonus = 1 THEN HY.HoursInYear ELSE p.BonusHoursToCollect END)
	       END AS BonusRate,
	       (SELECT SUM(CASE o.OverheadRateTypeId
	                       -- Multipliers
	                       WHEN 2 THEN
	                           (CASE
	                                 WHEN r.IsHourlyAmount = 1
	                                 THEN m.Amount
	                                 WHEN r.IsHourlyAmount = 0 OR s.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / s.HoursPerDay
	                             END) * o.Rate / 100
	                       WHEN 4 THEN p.HourlyRate * o.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN o.Rate * 12 / HY.HoursInYear
	                       ELSE o.Rate
	                   END)) AS OverheadRate,
	                   
			ISNULL((SELECT CASE MLFO.OverheadRateTypeId
	                       -- Multipliers
	                       WHEN 2 THEN
	                           (CASE
	                                 WHEN r.IsHourlyAmount = 1
	                                 THEN m.Amount
	                                 WHEN r.IsHourlyAmount = 0 OR s.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / s.HoursPerDay
	                             END) * MLFO.Rate / 100
	                       WHEN 4 THEN p.HourlyRate * MLFO.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN MLFO.Rate * 12 / HY.HoursInYear
	                       ELSE MLFO.Rate
						   END  
					   FROM dbo.v_MLFOverheadFixedRateTimescale MLFO 
					   WHERE MLFO.TimescaleId = p.Timescale
								AND r.Date >= MLFO.StartDate 
								AND (r.Date <=MLFO.EndDate OR MLFO.EndDate IS NULL)
								)
	                   ,0) MLFOverheadRate,
		   (CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * m.Amount
			ELSE p.HourlyRate END) * ISNULL(p.VacationDays,0)*m.HoursPerDay/HY.HoursInYear VacationRate,
	       ISNULL(MIN(pmcOwn.FractionOfMargin), 0) AS PracticeManagementCommissionOwn,
	       ISNULL(MIN(pmcSub.FractionOfMargin), 0) AS PracticeManagementCommissionSub,

	       (SELECT SUM(CASE
	                       WHEN DATEDIFF(DD, Person.HireDate, Calendar.Date)*8 <= rc.HoursToCollect
								AND rc.HoursToCollect > 0 
	                       THEN rc.Amount / (rc.HoursToCollect)
	                       ELSE NULL
	                   END)
	          FROM dbo.RecruiterCommission AS rc
	               INNER JOIN dbo.Person ON Person.PersonId = rc.RecruitId
	               INNER JOIN dbo.Calendar ON Calendar.Date = r.Date
	         WHERE rc.RecruitId = m.PersonId
	       ) AS RecruitingCommissionRate
	  FROM dbo.v_MilestoneRevenueRetrospective AS r
		   -- Linking to persons
	       LEFT JOIN dbo.v_MilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date
	       LEFT JOIN (SELECT s.Date, s.MilestoneId, SUM(HoursPerDay) AS HoursPerDay
						FROM dbo.v_MilestonePersonSchedule AS s
					  GROUP BY s.Date, s.MilestoneId) AS s
	           ON s.Date = r.Date AND s.MilestoneId = r.MilestoneId
	       -- Salary
		   LEFT JOIN dbo.v_PersonPayRetrospective AS p ON p.PersonId = m.PersonId AND p.Date = r.Date
	       LEFT JOIN dbo.v_OverheadFixedRateTimescale AS o
	           ON     p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, dbo.GetFutureDate())
	              AND o.Inactive = 0
	              AND o.TimescaleId = p.Timescale
	       LEFT JOIN (SELECT pmc.ProjectId, SUM(pmc.FractionOfMargin) AS FractionOfMargin
	                    FROM dbo.Commission AS pmc
	                   WHERE pmc.CommissionType = 2 AND MarginTypeId = 1
	                  GROUP BY pmc.ProjectId) AS pmcOwn ON pmcOwn.ProjectId = r.ProjectId
	       LEFT JOIN (SELECT pmc.ProjectId, SUM(pmc.FractionOfMargin) AS FractionOfMargin
	                    FROM dbo.Commission AS pmc
	                   WHERE pmc.CommissionType = 2 AND MarginTypeId = 2
	                  GROUP BY pmc.ProjectId) AS pmcSub ON pmcSub.ProjectId = r.ProjectId
		  LEFT JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(r.Date)
	GROUP BY r.Date, r.ProjectId, r.MilestoneId, r.MilestoneDailyAmount, r.Discount, p.HourlyRate,p.VacationDays,HY.HoursInYear,
	         m.Amount, p.BonusAmount,p.IsYearBonus, p.BonusHoursToCollect, p.Timescale, s.HoursPerDay,
	         r.IsHourlyAmount, m.HoursPerDay, m.PersonId, m.EntryStartDate, r.PracticeManagerId

