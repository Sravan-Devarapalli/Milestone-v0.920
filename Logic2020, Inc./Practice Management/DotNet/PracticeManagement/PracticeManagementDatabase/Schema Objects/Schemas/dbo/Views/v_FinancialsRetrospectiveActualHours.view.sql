CREATE VIEW [dbo].[v_FinancialsRetrospectiveActualHours]
AS
WITH ActualTimeEntries
AS 
(
	SELECT CC.ProjectId,
			TE.PersonId,
			TE.ChargeCodeDate,
			SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END) BillableHOursPerDay,
			SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END) NonBillableHoursPerDay,
			P.IsHourlyAmount
	FROM TimeEntry TE
	JOIN TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
	JOIN ChargeCode CC on CC.Id = TE.ChargeCodeId
	JOIN (
			SELECT Pro.ProjectId,CAST(CASE WHEN SUM(CAST(m.IsHourlyAmount as INT)) > 0 THEN 1 ELSE 0 END AS BIT) AS IsHourlyAmount
			FROM Project Pro 
				LEFT JOIN Milestone m ON m.ProjectId = Pro.ProjectId 
			WHERE Pro.IsAllowedToShow = 1
			GROUP BY Pro.ProjectId
		 ) P ON p.ProjectId = CC.ProjectId
	GROUP BY CC.ProjectId, TE.PersonId, TE.ChargeCodeDate,P.IsHourlyAmount
),
MileStoneEntries
AS
(
	SELECT  m.ProjectId,
			m.[MilestoneId],
			mp.PersonId,
			cal.Date,
			MPE.Id,
			MPE.Amount,
			m.IsHourlyAmount,	
			m.IsDefault,
			SUM(mpe.HoursPerDay) AS ActualHoursPerDay,
			-- dbo.PersonProjectedHoursPerDay(cal.DayOff,cal.companydayoff,cal.TimeoffHours,mpe.HoursPerDay) AS HoursPerDay,
			--Removed Inline Function for the sake of performance. When ever PersonProjectedHoursPerDay function is updated need to update below case when also.
			SUM(CONVERT(DECIMAL(4,2),CASE WHEN cal.DayOff = 0  THEN mpe.HoursPerDay -- No Time-off and no company holiday
				WHEN (cal.DayOff = 1 and cal.companydayoff = 1) OR (cal.DayOff = 1 AND cal.companydayoff = 0 AND ISNULL(cal.TimeoffHours,8) = 8) THEN 0 -- only company holiday OR person complete dayoff
				ELSE mpe.HoursPerDay * (1-(cal.TimeoffHours/8)) --person partial day off
			END)) AS HoursPerDay,
			SUM(CONVERT(DECIMAL(4,2),CASE WHEN cal.DayOff = 0  THEN mpe.HoursPerDay -- No Time-off and no company holiday
				WHEN (cal.DayOff = 1 and cal.companydayoff = 1) OR (cal.DayOff = 1 AND cal.companydayoff = 0 AND ISNULL(cal.TimeoffHours,8) = 8) THEN 0 -- only company holiday OR person complete dayoff
				ELSE mpe.HoursPerDay * (1-(cal.TimeoffHours/8)) --person partial day off
			END) * mpe.Amount) AS PersonMilestoneDailyAmount--PersonLevel
			FROM dbo.Project P 
			INNER JOIN dbo.[Milestone] AS m ON P.ProjectId=m.ProjectId AND p.IsAllowedToShow = 1
			INNER JOIN dbo.MilestonePerson AS mp ON m.[MilestoneId] = mp.[MilestoneId]
			INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
			INNER JOIN dbo.PersonCalendarAuto AS cal ON cal.Date BETWEEN mpe.Startdate AND mpe.EndDate AND cal.PersonId = mp.PersonId 
		    GROUP BY  m.ProjectId,m.[MilestoneId],mp.PersonId,cal.Date,m.IsHourlyAmount ,m.IsDefault,MPE.Id,MPE.Amount
)
SELECT	pro.ProjectId,
		Per.PersonId,
		c.Date,
		AE.BillableHOursPerDay,
		AE.NonBillableHoursPerDay,
		ISNULL(ME.IsHourlyAmount,AE.IsHourlyAmount) AS IsHourlyAmount,
		ME.HoursPerDay AS PersonHoursPerDay,
		ME.ActualHoursPerDay,
		r.Discount,
		r.HoursPerDay,
		0 AS PracticeManagementCommissionOwn,
		0 AS PracticeManagementCommissionSub,
		CASE
	           WHEN ME.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ME.PersonMilestoneDailyAmount
	           ELSE ISNULL(r.MilestoneDailyAmount * ME.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END AS PersonMilestoneDailyAmount,--Person Level Daily Amount
		   CASE
	           WHEN ME.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ISNULL(ME.PersonMilestoneDailyAmount * r.Discount / 100, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * ME.HoursPerDay * r.Discount / (r.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
	       END AS PersonDiscountDailyAmount, --Entry Level Daily Discount Amount
		 CASE
	           WHEN ME.IsHourlyAmount = 1
	           THEN ME.Amount
	           WHEN ME.IsHourlyAmount = 0 AND r.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount / r.HoursPerDay
		   END AS BillRate,
		     CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * ME.Amount
			ELSE p.HourlyRate
		   END AS PayRate, 	-- new payrate that takes into account that % unit is used in the Amount instead of $ unit
	       CASE p.BonusHoursToCollect
	           WHEN 0 THEN 0
	           ELSE p.BonusAmount / (CASE WHEN p.IsYearBonus = 1 THEN HY.HoursInYear ELSE p.BonusHoursToCollect END)
	       END AS BonusRate,
	        SUM(CASE o.OverheadRateTypeId
	                       -- Multipliers
	                       WHEN 2 THEN
	                           (CASE
	                                 WHEN r.IsHourlyAmount = 1
	                                 THEN ME.Amount
	                                 WHEN r.IsHourlyAmount = 0 OR r.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / r.HoursPerDay
	                             END) * o.Rate / 100 
	                       WHEN 4 THEN p.HourlyRate * o.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN (o.Rate * 12 / HY.HoursInYear) 
	                       ELSE o.Rate
	                   END) AS OverheadRate,

	           	ISNULL((CASE MLFO.OverheadRateTypeId
	                       -- Multipliers
	                       WHEN 2 THEN
	                           (CASE
	                                 WHEN ME.IsHourlyAmount = 1
	                                 THEN ME.Amount
	                                 WHEN ME.IsHourlyAmount = 0 OR r.HoursPerDay = 0
	                                 THEN 0
	                                 ELSE r.MilestoneDailyAmount / r.HoursPerDay
	                             END) * MLFO.Rate / 100
	                       WHEN 4 THEN p.HourlyRate * MLFO.Rate / 100
	                       -- Fixed
	                       WHEN 3 THEN (MLFO.Rate * 12 / HY.HoursInYear)
	                       ELSE MLFO.Rate 
						   END)
	                   ,0) MLFOverheadRate,
		      (CASE 
			WHEN p.Timescale = 4
			THEN p.HourlyRate * 0.01 * ME.Amount
			ELSE p.HourlyRate END) * ISNULL(p.VacationDays,0)* ME.HoursPerDay/HY.HoursInYear VacationRate
FROM ActualTimeEntries AS AE --ActualEntriesByPerson
		FULL JOIN MileStoneEntries AS ME ON ME.ProjectId = AE.ProjectId AND AE.PersonId = ME.PersonId AND ME.Date = AE.ChargeCodeDate 
		INNER JOIN dbo.Person Per ON per.PersonId = ISNULL(ME.PersonId,AE.PersonId)
		INNER JOIN dbo.Project Pro ON Pro.ProjectId = ISNULL(ME.ProjectId,AE.ProjectId) 
		INNER JOIN dbo.Calendar C ON c.Date = ISNULL(ME.Date,AE.ChargeCodeDate)
		INNER JOIN dbo.GetFutureDateTable() FD ON 1=1 --For improving query performance we are using table valued function instead of scalar function.
		LEFT JOIN dbo.[v_PersonPayRetrospective] AS p ON p.PersonId = per.PersonId AND p.Date = c.Date
		LEFT JOIN v_MLFOverheadFixedRateTimescale MLFO ON MLFO.TimescaleId = p.Timescale AND c.Date BETWEEN MLFO.StartDate AND ISNULL(MLFO.EndDate,FD.FutureDate)
		LEFT JOIN dbo.v_OverheadFixedRateTimescale AS o ON p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, FD.FutureDate) AND o.Inactive = 0 AND o.TimescaleId = p.Timescale
		LEFT JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(c.date)
		LEFT JOIN dbo.v_MilestoneRevenueRetrospective AS r ON ME.MilestoneId = r.MilestoneId AND c.Date = r.Date
	--	where pro.projectid = 666 AND per.PersonId = 3963 AND c.Date = '2013-02-04'
	GROUP BY pro.ProjectId,Per.PersonId,c.Date,AE.BillableHOursPerDay,AE.NonBillableHoursPerDay,ME.IsHourlyAmount,ME.HoursPerDay,ME.PersonMilestoneDailyAmount,
	p.Timescale,p.HourlyRate,p.BonusHoursToCollect,p.BonusAmount,p.IsYearBonus,HY.HoursInYear,p.VacationDays,
	MLFO.OverheadRateTypeId,r.HoursPerDay,r.MilestoneDailyAmount,r.Discount,MLFO.Rate,ME.Amount,ME.Id,ME.ActualHoursPerDay,AE.IsHourlyAmount
