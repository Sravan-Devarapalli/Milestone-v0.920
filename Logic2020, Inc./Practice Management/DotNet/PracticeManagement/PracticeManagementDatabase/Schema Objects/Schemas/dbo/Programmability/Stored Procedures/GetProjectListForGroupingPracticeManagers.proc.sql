CREATE PROCEDURE [dbo].[GetProjectListForGroupingPracticeManagers]
(
	@ClientIds			VARCHAR(250) = NULL,
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowInactive		BIT = 0,
	@SalespersonIds		VARCHAR(250) = NULL,
	@ProjectOwnerIds	VARCHAR(250) = NULL,
	@PracticeIds		VARCHAR(250) = NULL,
	@ProjectGroupIds	VARCHAR(250) = NULL,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@ExcludeInternalPractices BIT = 0
) WITH RECOMPILE
AS 
BEGIN
	SET NOCOUNT ON ;

	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

	-- Convert client ids from string to TABLE
	DECLARE @ClientsList TABLE (Id INT)
	INSERT INTO @ClientsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert practice ids from string to TABLE
	DECLARE @PracticesList TABLE (Id INT)
	INSERT INTO @PracticesList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	-- Convert project group ids from string to TABLE
	DECLARE @ProjectGroupsList TABLE (Id INT)
	INSERT INTO @ProjectGroupsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIds)

	-- Convert project owner ids from string to TABLE
	DECLARE @ProjectOwnersList TABLE (Id INT)
	INSERT INTO @ProjectOwnersList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIds)

	-- Convert salesperson ids from string to TABLE
	DECLARE @SalespersonsList TABLE (Id INT)
	INSERT INTO @SalespersonsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	DECLARE @TempProjectResult TABLE
									(ClientId INT,
									ProjectId INT,
									Name	  NVARCHAR(100),
									PracticeManagerId INT,
									PracticeId INT,
									StartDate DATETIME,
									EndDate	  DATETIME,
									ClientName NVARCHAR(100),
									PracticeName NVARCHAR(100),
									ProjectStatusId	INT,
									ProjectStatusName  NVARCHAR(25),
									ProjectNumber	NVARCHAR(12),
									GroupId			INT,
									GroupName		NVARCHAR(100),
									PracticeManagerFirstName	NVARCHAR(40),
									PracticeManagerLastName	NVARCHAR(40),
									DirectorId	INT,
									DirectorLastName		NVARCHAR(40),
									DirectorFirstName		NVARCHAR(40),
									[Discount]					DECIMAL (18, 2) 
									)
	INSERT INTO @TempProjectResult
	SELECT  p.ClientId,
			p.ProjectId,
			--p.Discount,
			--p.Terms,
			p.Name,
			p.PracticeManagerId,
			p.PracticeId,
			p.StartDate,
			p.EndDate,
			p.ClientName,
			p.PracticeName,
			p.ProjectStatusId,
			p.ProjectStatusName,
			p.ProjectNumber,
			p.GroupId,
			PG.Name GroupName,
		   pm.FirstName PracticeManagerFirstName,
		   pm.LastName PracticeManagerLastName,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   p.[Discount]
	FROM	dbo.v_Project AS p
	JOIN dbo.Person pm ON pm.PersonId = p.PracticeManagerId
	JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	left join dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	WHERE	    (c.CommissionType is NULL OR c.CommissionType = 1)
		    --AND ( (p.StartDate <= @EndDate AND p.EndDate <= @EndDate) OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ((p.StartDate <= @EndDate AND @StartDate <= p.EndDate ) OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR p.ClientId IN (select Id from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT Id from @ProjectGroupsList) )
			AND ( @ProjectOwnerIds IS NULL 
					OR EXISTS (SELECT 1 FROM dbo.ProjectManagers AS projManagers
								JOIN @ProjectOwnersList POL ON POL.Id = projManagers.ProjectManagerId
									WHERE projManagers.ProjectId = p.ProjectId
								)
					OR p.ProjectOwnerId IN (SELECT POL.Id  FROM @ProjectOwnersList POL)
			    )
			AND (    @SalespersonIds IS NULL 
				  OR c.PersonId IN (SELECT Id FROM @SalespersonsList)
			)
			AND (    ( @ShowProjected = 1 AND p.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND p.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND p.ProjectStatusId = 4 )
				  OR ( @showInternal = 1 AND p.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimental = 1 AND p.ProjectStatusId = 5 )
				  OR ( @ShowInactive = 1 AND p.ProjectStatusId = 1 ) -- Inactive
			)
			AND  (ISNULL(pr.IsCompanyInternal, 0) = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)				
			AND P.ProjectId <> @DefaultProjectId
			AND P.IsAllowedToShow = 1
	ORDER BY CASE p.ProjectStatusId
			   WHEN 2 THEN p.StartDate
			   ELSE p.EndDate
			 END

	SELECT * FROM @TempProjectResult

	SELECT 
		   PH.PracticeId,
		   PH.PracticeManagerId,
		   P.LastName PracticeManagerLastName,
		   P.FirstName PracticeManagerFirstName
		   --PH.StartDate,
		   --PH.EndDate
	FROM dbo.PracticeManagerHistory PH
	JOIN dbo.Person P ON P.PersonId = PH.PracticeManagerId
	WHERE (PracticeId IN (SELECT Id FROM @PracticesList) OR @PracticeIds IS NULL)
		  AND (StartDate BETWEEN @StartDate AND @EndDate OR EndDate BETWEEN @StartDate AND @EndDate
		  OR StartDate<= @StartDate AND EndDate>@StartDate)
		  AND EndDate IS NOT NULL
	ORDER BY PH.StartDate DESC
	
	;WITH CTEMilestonePersonSchedule
	AS
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
	       m.IsHourlyAmount,
		   mp.MilestonePersonId,
		   p.Discount
	  FROM @TempProjectResult P 
		   INNER JOIN dbo.[Milestone] AS m ON m.ProjectId=P.ProjectId 
		   INNER JOIN dbo.MilestonePerson AS mp ON mp.[MilestoneId] = m.[MilestoneId]
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.PersonCalendarAuto AS cal ON cal.Date BETWEEN mpe.Startdate AND mpe.EndDate AND mp.PersonId = cal.PersonId
		   WHERE cal.Date  BETWEEN @StartDate AND @EndDate

	),
	CTE 
	AS 
	(
		SELECT s.Date, s.MilestoneId, SUM(HoursPerDay) AS HoursPerDay
		FROM CTEMilestonePersonSchedule AS s
		WHERE s.IsHourlyAmount = 0
		GROUP BY s.Date, s.MilestoneId
	),
	CTEFinancialsRetroSpective
	AS
	(	
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
		   m.MilestonePersonId, 
	       m.HoursPerDay AS PersonHoursPerDay,--Entry level Hours Per Day
		   p.Timescale,
		   p.BonusRate,
		   p.PracticeId,
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
	  FROM (
			  SELECT -- Milestones with a fixed amount
				m.MilestoneId,
				m.ProjectId,
				cal.Date,
				m.IsHourlyAmount,
				ISNULL((m.Amount/ NULLIF(MTHours.TotalHours,0))* d.HoursPerDay,0) AS MilestoneDailyAmount /* ((Milestone fixed amount/Milestone Total  Hours)* Milestone Total  Hours per day)  */,
				p.Discount,
				HY.HoursInYear,
				d.HoursPerDay/* Milestone Total  Hours per day*/
			FROM @TempProjectResult P 
				INNER JOIN dbo.Milestone AS m ON m.ProjectId = p.ProjectId AND  m.IsHourlyAmount = 0
				INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN m.StartDate AND m.ProjectedDeliveryDate  AND cal.Date  BETWEEN @StartDate AND @EndDate
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
				   MAX(mp.Discount) AS Discount,
				   MAX(HY.HoursInYear) AS HoursInYear,
				   SUM(mp.HoursPerDay) AS HoursPerDay/* Milestone Total  Hours per day*/
			  FROM CTEMilestonePersonSchedule mp
				   INNER JOIN V_WorkinHoursByYear HY ON mp.date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate] AND mp.IsHourlyAmount = 1
			GROUP BY mp.MilestoneId, mp.ProjectId, mp.Date, mp.IsHourlyAmount
	  ) AS r
		   -- Linking to persons
	       INNER JOIN CTEMilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date
	       INNER JOIN dbo.GetFutureDateTable() FD ON 1=1 --For improving query performance we are using table valued function instead of scalar function.
	       -- Salary
		   LEFT JOIN (
					   SELECT p.Person AS Personid,
					   cal.Date,
					   p.Amount AS Rate,
					   p.Timescale,
					   p.PracticeId,
					   CASE
						   WHEN p.Timescale IN (1, 3, 4)
						   THEN p.Amount
						   ELSE p.Amount / HY.HoursInYear
					   END AS HourlyRate,
  					   p.BonusAmount / (CASE WHEN p.BonusHoursToCollect = GHY.HoursPerYear THEN HY.HoursInYear ELSE NULLIF(p.BonusHoursToCollect,0) END) AS BonusRate,
					   p.VacationDays
				  FROM dbo.PersonCalendarAuto AS cal 
					   INNER JOIN dbo.Pay AS p ON cal.PersonId = p.Person AND p.StartDate <= cal.Date AND p.EndDate > cal.date AND cal.Date  BETWEEN @StartDate AND @EndDate
					   INNER JOIN dbo.[BonusHoursPerYearTable]() GHY ON 1=1--For improving query performance we are using table valued function instead of scalar function.
					   INNER JOIN V_WorkinHoursByYear HY ON cal.Date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate]
					) AS p ON p.PersonId = m.PersonId AND p.Date = r.Date
		   LEFT JOIN v_MLFOverheadFixedRateTimescale MLFO ON MLFO.TimescaleId = p.Timescale
															AND p.Date BETWEEN MLFO.StartDate AND ISNULL(MLFO.EndDate, FD.FutureDate)
	       LEFT JOIN dbo.v_OverheadFixedRateTimescale AS o ON o.TimescaleId = p.Timescale 
															AND p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, FD.FutureDate)
	GROUP BY r.ProjectId, r.Discount, r.MilestoneId,r.IsHourlyAmount,r.Date,r.MilestoneDailyAmount,r.HoursPerDay,r.HoursInYear,
			 m.PersonId,m.EntryId, m.EntryStartDate,m.Amount,m.HoursPerDay, m.MilestonePersonId,
			 p.Timescale,p.PracticeId,p.HourlyRate,p.VacationDays,p.BonusRate,MLFO.Rate
	),
	  ProjectExpensesMonthly
		AS
		(
			SELECT pexp.ProjectId,
				CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
				CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Reimbursement,
				c.MonthStartDate AS FinancialDate,
				c.MonthEndDate AS MonthEnd
			FROM dbo.ProjectExpense as pexp
			JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
			WHERE ProjectId IN (SELECT ProjectId FROM @TempProjectResult) AND c.Date BETWEEN @StartDate	AND @EndDate
			GROUP BY pexp.ProjectId,c.MonthStartDate,c.MonthEndDate
		) 

		SELECT temp.ProjectId,
			   temp.PersonId,
			   temp.MilestonePersonId,
			   temp.PracticeId,
			   temp.PracticeName,
			   temp.PracticeManagerLastName,
			   temp.PracticeManagerFirstName,
			   temp.PracticeManagerId,
			   temp.MilestoneId,
			   temp.MilestoneName,
			   temp.MilestonePersonFirstName,
			   temp.MilestonePersonLastName,
			   temp.MonthStartDate,
			   temp.MonthEndDate,
			   temp.Revenue AS 'Revenue',
			   temp.GrossMargin+(ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0))* (1 - temp.Discount/100)  as 'GrossMargin' 
		FROM
		(
		SELECT f.ProjectId,
			   f.PersonId,
			   f.MilestonePersonId,
			   f.PracticeId,
			   pra.Name PracticeName,
			   PM.LastName PracticeManagerLastName,
			   PM.FirstName PracticeManagerFirstName,
			   pra.PracticeManagerId,
			   f.MilestoneId,
			   mile.Description MilestoneName,
			   per.FirstName MilestonePersonFirstName,
			   per.LastName MilestonePersonLastName,
			   cal.MonthStartDate AS MonthStartDate,
			   cal.MonthEndDate AS MonthEndDate,
			   ISNULL(SUM(f.PersonMilestoneDailyAmount),0) AS Revenue,
			   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount-
							(CASE WHEN f.SLHR  >=  f.PayRate +f.MLFOverheadRate 
								  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
							*ISNULL(f.PersonHoursPerDay, 0)),0) GrossMargin,
				MAX(f.Discount) Discount
		  FROM (
	  
				SELECT f.ProjectId,
					   f.MilestoneId,
					   f.MilestonePersonId,
					   f.PracticeId,
					   f.Date, 
					   f.PersonMilestoneDailyAmount,
					   f.PersonDiscountDailyAmount,
					   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
					   ISNULL(f.PayRate,0) PayRate,
					   f.MLFOverheadRate,
					   f.PersonHoursPerDay,
					   f.PersonId,
					   f.Discount,
					   f.EntryId
				FROM CTEFinancialsRetroSpective f 
			 ) as f
		  INNER JOIN dbo.Calendar cal ON f.Date = cal.Date
		  INNER JOIN dbo.MilestonePerson MP ON MP.MilestoneId = f.MilestoneId AND MP.PersonId = f.PersonId 
		  INNER JOIN dbo.MilestonePersonEntry MPE ON MP.MilestonePersonId = MPE.MilestonePersonId  AND f.EntryId = MPE.Id 
											AND f.Date BETWEEN MPE.StartDate AND MPE.EndDate
		  INNER JOIN dbo.Milestone mile ON mile.MilestoneId = f.MilestoneId
		  INNER JOIN dbo.Practice pra ON pra.PracticeId = f.PracticeId
		  INNER JOIN dbo.Person PM ON PM.PersonId = pra.PracticeManagerId
		  INNER JOIN dbo.Person per ON per.PersonId = f.PersonId	  
		  WHERE  f.PersonId IS NOT NULL 
			AND ( @PracticeIds IS NULL 
					OR f.PracticeId IN (SELECT Id FROM @PracticesList)) 
		  GROUP BY f.ProjectId,f.MilestonePersonId, f.PersonId,f.PracticeId, pra.Name,
					PM.LastName,PM.FirstName,pra.PracticeManagerId,f.MilestoneId,mile.Description,
					per.FirstName,per.LastName,cal.MonthStartDate,cal.MonthEndDate
	  ) Temp
	  LEFT JOIN ProjectExpensesMonthly  PEM 
	ON PEM.ProjectId = Temp.ProjectId AND Temp.MonthStartDate = PEM.FinancialDate  AND Temp.MonthEndDate = PEM.MonthEnd
	ORDER BY Temp.MonthStartDate

END

