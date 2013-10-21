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


	;WITH
	cteMilestonePersonSchedule as
	(
	SELECT m.[MilestoneId],
	       mp.PersonId,
	      -- dbo.PersonProjectedHoursPerDay(cal.DayOff,cal.companydayoff,cal.TimeoffHours,mpe.HoursPerDay) AS HoursPerDay,
		  --Removed Inline Function for the sake of performance. When ever PersonProjectedHoursPerDay function is updated need to update below case when also.
			CONVERT(DECIMAL(4,2),CASE WHEN cal.DayOff = 0  THEN mpe.HoursPerDay -- No Time-off and no company holiday
				WHEN (cal.DayOff = 1 and cal.companydayoff = 1) OR (cal.DayOff = 1 AND cal.companydayoff = 0 AND ISNULL(cal.TimeoffHours,8) = 8) THEN 0 -- only company holiday OR person complete dayoff
				ELSE mpe.HoursPerDay * (1-(cal.TimeoffHours/8)) --person partial day off
			END) AS HoursPerDay,
	       cal.Date,
	       m.ProjectId,
	       mpe.Id EntryId,
		   mpe.StartDate AS EntryStartDate,
	       mpe.Amount,
	       mpe.PersonRoleId,
	       m.IsHourlyAmount
	  FROM dbo.Project P 
		   INNER JOIN dbo.[Milestone] AS m ON m.ProjectId=P.ProjectId AND P.IsAdministrative = 0 AND P.ProjectId != 174  AND m.IsDefault = 0
		   INNER JOIN dbo.MilestonePerson AS mp ON mp.[MilestoneId] = m.[MilestoneId]
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.PersonCalendarAuto AS cal ON cal.Date BETWEEN mpe.Startdate AND mpe.EndDate AND cal.PersonId = @PersonIdLocal AND mp.PersonId = cal.PersonId 
	), CTE 
	AS 
	(
		SELECT s.Date, s.MilestoneId, SUM(HoursPerDay) AS HoursPerDay
		FROM v_MilestonePersonSchedule AS s
		WHERE s.IsHourlyAmount = 0
		GROUP BY s.Date, s.MilestoneId
	),
	cteMilestoneRevenueRetrospective as
	(
		SELECT -- Milestones with a fixed amount
		m.MilestoneId,
		m.ProjectId,
		cal.Date,
		m.IsHourlyAmount,
		ISNULL((m.Amount/ NULLIF(MTHours.TotalHours,0))* d.HoursPerDay,0) AS MilestoneDailyAmount /* ((Milestone fixed amount/Milestone Total  Hours)* Milestone Total  Hours per day)  */,
		p.Discount,
		HY.HoursInYear,
		d.HoursPerDay/* Milestone Total  Hours per day*/
	FROM dbo.Project AS p 
		INNER JOIN dbo.Milestone AS m ON m.ProjectId = p.ProjectId AND P.IsAdministrative = 0 AND P.ProjectId != 174 AND  m.IsHourlyAmount = 0
		INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN m.StartDate AND m.ProjectedDeliveryDate
		INNER JOIN (
						SELECT s.MilestoneId, SUM(s.HoursPerDay) AS TotalHours
						FROM CTE AS s 
						GROUP BY s.MilestoneId
					) AS MTHours  ON MTHours.MilestoneId  = m.MilestoneId
		INNER JOIN CTE AS d ON d.date = cal.Date and m.MilestoneId = d.MileStoneId
		INNER JOIN V_WorkinHoursByYear HY ON cal.date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate]
	UNION ALL
	SELECT -- Milestones with a hourly amount
		   mp.MilestoneId,
		   mp.ProjectId,
		   mp.Date,
		   mp.IsHourlyAmount,
		   ISNULL(SUM(mp.Amount * mp.HoursPerDay), 0) AS MilestoneDailyAmount,
           MAX(p.Discount) AS Discount,
		   MAX(HY.HoursInYear) AS HoursInYear,
	       SUM(mp.HoursPerDay) AS HoursPerDay/* Milestone Total  Hours per day*/
	  FROM cteMilestonePersonSchedule mp
	       INNER JOIN dbo.Project AS p ON mp.ProjectId = p.ProjectId AND mp.IsHourlyAmount = 1
		   INNER JOIN V_WorkinHoursByYear HY ON mp.date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate]
	GROUP BY mp.MilestoneId, mp.ProjectId, mp.Date, mp.IsHourlyAmount
	),CTEPersonPayRetrospective 
	AS
	(
	SELECT cal.PersonId,
	       cal.Date,
	       p.Timescale,
	       CASE
	           WHEN p.Timescale IN (1, 3, 4)
	           THEN p.Amount
	           ELSE p.Amount / HY.HoursInYear
	       END AS HourlyRate,
  		   p.BonusAmount / (CASE WHEN p.BonusHoursToCollect = GHY.HoursPerYear THEN HY.HoursInYear ELSE NULLIF(p.BonusHoursToCollect,0) END) AS BonusRate,
		   p.VacationDays
	  FROM dbo.PersonCalendarAuto AS cal
	       INNER JOIN dbo.Pay AS p ON cal.PersonId = p.Person AND p.StartDate <= cal.Date AND p.EndDate > cal.date  
		   INNER JOIN dbo.[BonusHoursPerYearTable]() GHY ON 1=1--For improving query performance we are using table valued function instead of scalar function.
	       INNER JOIN V_WorkinHoursByYear HY ON cal.Date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate]
	       WHERE cal.PersonId = @PersonIdLocal
	       )
	,cteMLFOverheadFixedRateTimescale as
	(
		SELECT	MH.Rate,
			MH.StartDate,
			MH.EndDate,
			MH.TimescaleId
	FROM dbo.OverheadFixedRate AS o
	JOIN dbo.MinimumLoadFactorHistory AS MH  ON MH.OverheadFixedRateId = o.OverheadFixedRateId 
												AND o.IsMinimumLoadFactor = 1 
												AND o.Inactive = 0 
												AND MH.Rate > 0 
												AND o.RateType = 4 --Pay Rate Multiplier
	),cteOverheadFixedRateTimescale as
	(
		SELECT	ofr.Rate,
			ofr.StartDate,
			ofr.EndDate,
			ofr.RateType AS OverheadRateTypeId,
			ortt.TimescaleId
	FROM dbo.OverheadFixedRate AS ofr
		INNER JOIN dbo.OverheadFixedRateTimescale AS ortt ON ortt.OverheadFixedRateId = ofr.OverheadFixedRateId AND ofr.IsMinimumLoadFactor = 0 AND ofr.Inactive = 0
	)
	,cteFinancialsRetrospective as
	(
	SELECT r.ProjectId,
		   r.MilestoneId,
		   r.Date,
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
		   m.PersonId,
		   m.EntryId,
	       m.HoursPerDay AS PersonHoursPerDay,--Entry level Hours Per Day
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
	  FROM cteMilestoneRevenueRetrospective AS r
		   -- Linking to persons
	       INNER JOIN cteMilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date  
	       INNER JOIN dbo.GetFutureDateTable() FD ON 1=1 --For improving query performance we are using table valued function instead of scalar function.
	       -- Salary
		   LEFT JOIN CTEPersonPayRetrospective AS p ON p.PersonId = m.PersonId AND p.Date = r.Date 
		   LEFT JOIN cteMLFOverheadFixedRateTimescale MLFO ON MLFO.TimescaleId = p.Timescale
															AND p.Date BETWEEN MLFO.StartDate AND ISNULL(MLFO.EndDate, FD.FutureDate)
	       LEFT JOIN cteOverheadFixedRateTimescale AS o ON o.TimescaleId = p.Timescale 
															AND p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, FD.FutureDate)
				where		m.PersonId = @PersonIdLocal						
	GROUP BY r.ProjectId, r.Discount, r.MilestoneId,r.IsHourlyAmount,r.Date,r.MilestoneDailyAmount,r.HoursPerDay,r.HoursInYear,
			 m.PersonId,m.EntryId, m.EntryStartDate,m.Amount,m.HoursPerDay,
			 p.Timescale,p.HourlyRate,p.VacationDays,p.BonusRate,MLFO.Rate
	)
	,
	 FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.MilestoneId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.Discount,
		   f.EntryId
	FROM cteFinancialsRetrospective f
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

