CREATE VIEW [dbo].[v_FinancialsRetrospective]
AS
	SELECT r.ProjectId,
		   r.MilestoneId,
		   r.Date,
		   r.IsHourlyAmount,
	       CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ISNULL(m.Amount*m.HoursPerDay, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END AS PersonMilestoneDailyAmount,--Entry Level Daily Amount
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
		   p.Timescale,
		   p.BonusRate,
		   CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * m.Amount
			ELSE p.HourlyRate
		   END AS PayRate,
	       SUM(CASE o.OverheadRateTypeId
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
	                       WHEN 3 THEN o.Rate * 12 / r.HoursInYear 
	                       ELSE o.Rate
	                   END) AS OverheadRate,
			ISNULL(p.HourlyRate * MLFO.Rate / 100,0) MLFOverheadRate,
			--Vacation days are allowed only for w2 salary person
		   (CASE WHEN p.Timescale = 2
				 THEN ISNULL(p.HourlyRate * p.VacationDays * m.HoursPerDay,0)/r.HoursInYear
			ELSE 0 END)  VacationRate
	  FROM dbo.v_MilestoneRevenueRetrospective AS r
		   -- Linking to persons
	       INNER JOIN dbo.v_MilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date
	       INNER JOIN dbo.GetFutureDateTable() FD ON 1=1 --For improving query performance we are using table valued function instead of scalar function.
	       -- Salary
		   LEFT JOIN dbo.v_PersonPayRetrospective AS p ON p.PersonId = m.PersonId AND p.Date = r.Date
		   LEFT JOIN v_MLFOverheadFixedRateTimescale MLFO ON MLFO.TimescaleId = p.Timescale
															AND p.Date BETWEEN MLFO.StartDate AND ISNULL(MLFO.EndDate, FD.FutureDate)
	       LEFT JOIN dbo.v_OverheadFixedRateTimescale AS o ON o.TimescaleId = p.Timescale 
															AND p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, FD.FutureDate)
	GROUP BY r.ProjectId, r.Discount, r.MilestoneId,r.IsHourlyAmount,r.Date,r.MilestoneDailyAmount,r.HoursPerDay,r.HoursInYear,
			 m.PersonId,m.EntryId, m.EntryStartDate,m.Amount,m.HoursPerDay,
			 p.Timescale,p.HourlyRate,p.VacationDays,p.BonusRate,MLFO.Rate
