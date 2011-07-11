CREATE PROCEDURE [dbo].[GetProjectListForGroupingPracticeManagers]
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
AS 
	SET NOCOUNT ON ;

	SET NOCOUNT ON ;

	-- Convert client ids from string to table
	declare @ClientsList table (Id int)
	insert into @ClientsList
	select * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert practice ids from string to table
	declare @PracticesList table (Id int)
	insert into @PracticesList
	select * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	-- Convert project group ids from string to table
	declare @ProjectGroupsList table (Id int)
	insert into @ProjectGroupsList
	select * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIds)

	-- Convert project owner ids from string to table
	declare @ProjectOwnersList table (Id int)
	insert into @ProjectOwnersList
	select * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIds)

	-- Convert salesperson ids from string to table
	declare @SalespersonsList table (Id int)
	insert into @SalespersonsList
	select * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)
	union all
	-- All persons with Role = Salesperson
	select PersonId from v_UsersInRoles where RoleName = 'Salesperson'
	union all
	-- All persons that have commission
	SELECT c.PersonId FROM dbo.DefaultCommission AS c
    WHERE [type] = 1 AND dbo.IsDateRangeWithingTimeInterval(c.StartDate, c.EndDate, @StartDate, @EndDate) = 1
	union all
	-- All persons that have project commission
	SELECT com.PersonId FROM dbo.Commission AS com
    WHERE com.CommissionType = 1

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	DECLARE @TempProjectResult table
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
									DirectorFirstName		NVARCHAR(40)
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
		   p.DirectorFirstName
	FROM	dbo.v_Project AS p
	JOIN dbo.Person pm ON pm.PersonId = p.PracticeManagerId
	JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	left join dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	WHERE	    (c.CommissionType is NULL OR c.CommissionType = 1)
		    AND (dbo.IsDateRangeWithingTimeInterval(p.StartDate, p.EndDate, @StartDate, @EndDate) = 1 OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR p.ClientId IN (select Id from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT Id from @ProjectGroupsList) )
			AND ( @ProjectOwnerIds IS NULL OR p.ProjectManagerId IN (SELECT Id FROM @ProjectOwnersList) )
			AND (    @SalespersonIds IS NULL 
				  OR c.PersonId IN (SELECT Id FROM @SalespersonsList)
				  OR c.PersonId is null
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
	  SELECT  m.[MilestoneId],
			mp.PersonId,
			m.ProjectId,
			m.IsHourlyAmount,
			mpe.MilestonePersonId,
			CASE
			WHEN cal.Date BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, m.[ProjectedDeliveryDate])
			THEN mpe.HoursPerDay
			ELSE 0
			END AS HoursPerDay,
			cal.Date,
			mpe.StartDate AS EntryStartDate,
			mpe.Amount
	   FROM dbo.[Milestone] AS m
	   INNER JOIN dbo.MilestonePerson AS mp ON m.[MilestoneId] = mp.[MilestoneId]
	   INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId			   
	   INNER JOIN dbo.PersonCalendarAuto AS cal ON (cal.Date BETWEEN mpe.Startdate AND ISNULL(mpe.EndDate, m.ProjectedDeliveryDate) AND cal.PersonId = mp.PersonId)
	   WHERE cal.DayOff = 0 
	),
	CTEFinancialsRetroSpective
	AS
	(	  	  
			  SELECT r.ProjectId,
				   r.MilestoneId,
				   r.Date,
				   p.PracticeId,
				   r.Discount,
				   CASE
					   WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
					   THEN ISNULL(m.Amount*m.HoursPerDay, 0)
					   ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / s.HoursPerDay, r.MilestoneDailyAmount)
				   END AS PersonMilestoneDailyAmount,
				   CASE
					   WHEN r.IsHourlyAmount = 1 OR s.HoursPerDay = 0
					   THEN ISNULL(m.Amount * m.HoursPerDay * r.Discount / 100, 0)
					   ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay * r.Discount / (s.HoursPerDay * 100), r.MilestoneDailyAmount * r.Discount / 100)
				   END AS PersonDiscountDailyAmount,
				   m.PersonId,
				   m.MilestonePersonId,
				   m.EntryStartDate,
				   m.HoursPerDay AS PersonHoursPerDay,
				   CASE 
					WHEN p.Timescale = 4
					THEN p.HourlyRate * 0.01 * m.Amount
					ELSE p.HourlyRate
				   END AS PayRate, 	-- new payrate that takes into account that % unit is used in the Amount instead of $ unit
				   CASE p.BonusHoursToCollect
					WHEN 0 THEN 0
					ELSE p.BonusAmount / p.BonusHoursToCollect
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
	                   END  FROM dbo.v_MLFOverheadFixedRateTimescale MLFO 
					   WHERE MLFO.TimescaleId = p.Timescale
								AND r.Date >= MLFO.StartDate 
								AND (r.Date <=MLFO.EndDate OR MLFO.EndDate IS NULL))
	                   ,0) MLFOverheadRate,
				   (CASE 
				   WHEN p.Timescale = 4
				   THEN p.HourlyRate * 0.01 * m.Amount
				   ELSE p.HourlyRate END) * ISNULL(p.VacationDays,0)*m.HoursPerDay/HY.HoursInYear VacationRate,
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

			  FROM  (			  
						  SELECT -- Milestones with a fixed amount
							   m.MilestoneId,
							   m.ProjectId,
							   cal.Date,
							   m.IsHourlyAmount,
							   ISNULL((m.Amount / (SELECT  SUM(HoursPerDay)
												   FROM CTEMilestonePersonSchedule m1 WHERE  m1.MileStoneId = m.MilestoneId
											)) * ISNULL(d.HoursPerDay, 0),
									  (CASE (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
										   WHEN 0 THEN 0
										   ELSE m.Amount / (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
									   END)) AS MilestoneDailyAmount,
							   p.Discount
						  FROM dbo.Milestone AS m
							   INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN m.StartDate AND m.ProjectedDeliveryDate
							   INNER JOIN dbo.Project AS p ON m.ProjectId = p.ProjectId
							   INNER JOIN dbo.Practice AS prac ON p.PracticeId = prac.PracticeId
							   LEFT JOIN (
										   SELECT  ps1.[MilestoneId],
											SUM(ps1.HoursPerDay )   HoursPerDay,
											ps1.Date
											FROM  CTEMilestonePersonSchedule ps1
										GROUP BY ps1.Date,ps1.MilestoneId
					           
							   ) d ON d.date = cal.Date and m.MilestoneId = d.MileStoneId
						WHERE m.IsHourlyAmount = 0
						 
						UNION ALL

						SELECT ps2.[MilestoneId],
								ps2.ProjectId,
								ps2.Date,
							   ps2.IsHourlyAmount,
							   ISNULL(SUM(ps2.Amount *( ps2.HoursPerDay )),0) MilestoneDailyAmount,
							   MAX(p.Discount) AS Discount
						  FROM CTEMilestonePersonSchedule ps2
							   INNER JOIN dbo.Project AS p ON ps2.ProjectId = p.ProjectId
							   INNER JOIN dbo.Practice AS prac ON p.PracticeId = prac.PracticeId
						 WHERE ps2.IsHourlyAmount = 1
						GROUP BY ps2.MilestoneId, ps2.ProjectId, ps2.Date, ps2.IsHourlyAmount, prac.PracticeManagerId
					) AS r
				   -- Linking to persons
				   LEFT JOIN  CTEMilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date
				   LEFT JOIN (
									SELECT  ps3.[MilestoneId],
											SUM(ps3.HoursPerDay )   HoursPerDay,
											ps3.Date
									FROM  CTEMilestonePersonSchedule ps3
									GROUP BY ps3.Date,ps3.MilestoneId
							  ) AS s  ON s.Date = r.Date AND s.MilestoneId = r.MilestoneId 
				   -- Salary
				   LEFT JOIN (
				   
						   SELECT		cal.PersonId,
							cal.Date,
							p.Timescale,
							CASE
							   WHEN p.Timescale IN (1, 3, 4)
							   THEN p.Amount
							   ELSE p.Amount / HY1.HoursInYear
							END AS HourlyRate,
							p.BonusAmount,
							p.BonusHoursToCollect,
							p.PracticeId,
							p.VacationDays
					FROM dbo.PersonCalendarAuto AS cal
					INNER JOIN dbo.Pay AS p ON p.StartDate <= cal.Date 
												AND p.EndDate > cal.date 
												AND cal.PersonId = p.Person
					LEFT JOIN V_WorkinHoursByYear HY1 ON HY1.[Year] = YEAR(cal.Date)
					WHERE cal.DayOff = 0
					 AND cal.Date between @StartDate and @EndDate
				   )AS p ON p.PersonId = m.PersonId AND p.Date = r.Date
				   LEFT JOIN (
								SELECT r.Rate,
									r.StartDate,
									r.EndDate,
									r.Inactive,
									r.RateType OverheadRateTypeId,
									r.IsCogs,
									t.TimescaleId
								FROM  dbo.OverheadFixedRate AS r
								INNER JOIN dbo.OverheadFixedRateTimescale AS t ON r.OverheadFixedRateId = t.OverheadFixedRateId
						   ) AS o ON p.Date BETWEEN o.StartDate AND ISNULL(o.EndDate, dbo.GetFutureDate())
									  AND o.Inactive = 0
									  AND o.TimescaleId = p.Timescale
				 LEFT JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(r.Date)
			GROUP BY r.Date, r.ProjectId, r.MilestoneId, r.MilestoneDailyAmount, r.Discount, p.HourlyRate,p.VacationDays,HY.HoursInYear,
					 m.Amount, p.BonusAmount, p.BonusHoursToCollect, p.Timescale,p.PracticeId, s.HoursPerDay,
					 r.IsHourlyAmount, m.HoursPerDay, m.PersonId,m.MilestonePersonId, m.EntryStartDate

	  ),
	  ProjectExpensesMonthly
		AS
		(
			SELECT pexp.ProjectId,
				CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
				CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Reimbursement,
				dbo.MakeDate(YEAR(MIN(c.Date)), MONTH(MIN(c.Date)), 1) AS FinancialDate,
			   dbo.MakeDate(YEAR(MIN(c.Date)), MONTH(MIN(c.Date)), dbo.GetDaysInMonth(MIN(C.Date))) AS MonthEnd
			FROM dbo.ProjectExpense as pexp
			JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
			WHERE ProjectId IN (SELECT ProjectId FROM @TempProjectResult) AND c.Date BETWEEN @StartDate	AND @EndDate
			GROUP BY pexp.ProjectId,MONTH(c.Date),YEAR(c.Date)
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
			   temp.Revenue +ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0) AS 'Revenue',
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
			   dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS MonthStartDate,
			   dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEndDate,
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
					   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
						+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
					   ISNULL(f.PayRate,0) PayRate,
					   f.MLFOverheadRate,
					   f.PersonHoursPerDay,
					   --f.PracticeManagementCommissionSub,
					   --f.PracticeManagementCommissionOwn ,
					   --f.PracticeManagerId,
					   f.PersonId
					   , f.Discount
				FROM CTEFinancialsRetroSpective f 
			 ) as f
		  JOIN MilestonePerson MP ON MP.MilestoneId = f.MilestoneId AND MP.PersonId = f.PersonId 
		  JOIN MilestonePersonEntry MPE ON MP.MilestonePersonId = MPE.MilestonePersonId 
											AND f.Date BETWEEN MPE.StartDate AND MPE.EndDate
		  JOIN dbo.Milestone mile ON mile.MilestoneId = f.MilestoneId
		  JOIN dbo.Practice pra ON pra.PracticeId = f.PracticeId
		  JOIN dbo.Person PM ON PM.PersonId = pra.PracticeManagerId
		  JOIN dbo.Person per ON per.PersonId = f.PersonId	  
		  JOIN @TempProjectResult pr ON pr.ProjectId = f.ProjectId 
		  WHERE  f.PersonId IS NOT NULL 
			AND ( @PracticeIds IS NULL 
					OR f.PracticeId IN (SELECT Id FROM @PracticesList)) 
					AND f.Date BETWEEN @StartDate AND @EndDate
		  GROUP BY f.ProjectId,f.MilestonePersonId, f.PersonId,f.PracticeId, pra.Name,
					PM.LastName,PM.FirstName,pra.PracticeManagerId,f.MilestoneId,mile.Description,
					per.FirstName,per.LastName,YEAR(f.Date), MONTH(f.Date)
	  ) Temp
	  LEFT JOIN ProjectExpensesMonthly  PEM 
	ON PEM.ProjectId = Temp.ProjectId AND Temp.MonthStartDate = PEM.FinancialDate  AND Temp.MonthEndDate = PEM.MonthEnd
	ORDER BY Temp.MonthStartDate
