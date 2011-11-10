CREATE VIEW [dbo].[v_FinancialsRetrospective]
AS
	SELECT r.ProjectId,
		   r.MilestoneId,
	       r.PracticeManagerId,
		   r.Date,
		   r.IsHourlyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN r.MilestoneDailyAmount
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END AS MilestoneDailyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ISNULL(m.Amount*m.HoursPerDay, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END AS PersonMilestoneDailyAmount,--Entry Level Daily Amount
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN r.MilestoneDailyAmount * r.Discount / 100
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay * r.Discount / (r.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
	       END AS DiscountDailyAmount,
		   CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ISNULL(m.Amount * m.HoursPerDay * r.Discount / 100, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay * r.Discount / (r.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
	       END AS PersonDiscountDailyAmount, --Entry Level Daily Discount Amount
	       r.Discount,
		   r.HoursPerDay,
		   m.PersonId,
		   m.EntryId,
	       m.EntryStartDate,
	       m.HoursPerDay AS PersonHoursPerDay,--Entry level Hours Per Day
		   CASE
	           WHEN r.IsHourlyAmount = 1
	           THEN m.Amount
	           WHEN r.IsHourlyAmount = 0 AND r.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount / r.HoursPerDay
		   END AS BillRate,
		   CASE
	           WHEN r.IsHourlyAmount = 1
	           THEN m.Amount * r.Discount / 100
	           WHEN r.IsHourlyAmount = 0 AND r.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount * r.Discount / (r.HoursPerDay * 100)
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
	                                 WHEN r.IsHourlyAmount = 0 OR r.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / r.HoursPerDay
	                             END) * o.Rate / 100 
	                       WHEN 4 THEN p.HourlyRate * o.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN o.Rate * 12 / HY.HoursInYear 
	                       ELSE o.Rate
	                   END)) AS OverheadRate,
	                   
			ISNULL((CASE MLFO.OverheadRateTypeId
	                       -- Multipliers
	                       WHEN 2 THEN
	                           (CASE
	                                 WHEN r.IsHourlyAmount = 1
	                                 THEN m.Amount
	                                 WHEN r.IsHourlyAmount = 0 OR r.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / r.HoursPerDay
	                             END) * MLFO.Rate / 100
	                       WHEN 4 THEN p.HourlyRate * MLFO.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN MLFO.Rate * 12 / HY.HoursInYear
	                       ELSE MLFO.Rate
						   END)
	                   ,0) MLFOverheadRate,
		   (CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * m.Amount
			ELSE p.HourlyRate END) * ISNULL(p.VacationDays,0)*m.HoursPerDay/HY.HoursInYear VacationRate,
		   0 AS PracticeManagementCommissionOwn,
	       0 AS PracticeManagementCommissionSub,
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
	       JOIN dbo.v_MilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date 
	       -- Salary
		   LEFT JOIN dbo.v_PersonPayRetrospective AS p ON p.PersonId = m.PersonId AND p.Date = r.Date
	       LEFT JOIN dbo.v_OverheadFixedRateTimescale AS o
	           ON     p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, dbo.GetFutureDate())
	              AND o.Inactive = 0
	              AND o.TimescaleId = p.Timescale
		  LEFT JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(r.Date)
		  LEFT JOIN v_MLFOverheadFixedRateTimescale MLFO ON MLFO.TimescaleId = p.Timescale
								AND r.Date >= MLFO.StartDate 
								AND (r.Date <=MLFO.EndDate OR MLFO.EndDate IS NULL)
	GROUP BY r.Date, r.ProjectId, r.MilestoneId, r.MilestoneDailyAmount, r.Discount, p.HourlyRate,p.VacationDays,HY.HoursInYear,
	         m.Amount, p.BonusAmount,p.IsYearBonus, p.BonusHoursToCollect, p.Timescale, r.HoursPerDay,
	         r.IsHourlyAmount, m.HoursPerDay, m.PersonId,m.EntryId, m.EntryStartDate, r.PracticeManagerId,MLFO.OverheadRateTypeId,MLFO.Rate

