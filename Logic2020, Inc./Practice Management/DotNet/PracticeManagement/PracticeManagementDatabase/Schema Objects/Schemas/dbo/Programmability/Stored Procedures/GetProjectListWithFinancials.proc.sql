CREATE PROCEDURE [dbo].[GetProjectListWithFinancials]
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
)WITH RECOMPILE
AS 
BEGIN
	SET NOCOUNT ON ;

	DECLARE 
	@ClientIdsLocal			VARCHAR(250) = NULL,
	@ShowProjectedLocal		BIT = 0,
	@ShowCompletedLocal		BIT = 0,
	@ShowActiveLocal			BIT = 0,
	@showInternalLocal		BIT = 0,
	@ShowExperimentalLocal	BIT = 0,
	@ShowInactiveLocal		BIT = 0,
	@SalespersonIdsLocal		VARCHAR(250) = NULL,
	@ProjectOwnerIdsLocal	VARCHAR(250) = NULL,
	@PracticeIdsLocal		VARCHAR(250) = NULL,
	@ProjectGroupIdsLocal	VARCHAR(250) = NULL,
	@StartDateLocal			DATETIME,
	@EndDateLocal			DATETIME,
	@ExcludeInternalPracticesLocal BIT = 0

	SELECT @ClientIdsLocal	= @ClientIds,
	@ShowProjectedLocal		= @ShowProjected,
	@ShowCompletedLocal		=@ShowCompleted,
	@ShowActiveLocal		=@ShowActive,
	@showInternalLocal		=@showInternal,
	@ShowExperimentalLocal	=@ShowExperimental,
	@ShowInactiveLocal		=@ShowInactive,
	@SalespersonIdsLocal	=@SalespersonIds,
	@ProjectOwnerIdsLocal	=@ProjectOwnerIds,
	@PracticeIdsLocal		=@PracticeIds,
	@ProjectGroupIdsLocal	=@ProjectGroupIds,
	@StartDateLocal			=@StartDate,
	@EndDateLocal			=@EndDate,
	@ExcludeInternalPracticesLocal =@ExcludeInternalPractices


	DECLARE @FutureDate DATETIME
	SELECT @FutureDate = dbo.GetFutureDate()

	-- Convert client ids from string to TABLE
	DECLARE @ClientsList TABLE (Id INT)
	INSERT INTO @ClientsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ClientIdsLocal)

	-- Convert practice ids from string to TABLE
	DECLARE @PracticesList TABLE (Id INT)
	INSERT INTO @PracticesList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIdsLocal)

	-- Convert project group ids from string to TABLE
	DECLARE @ProjectGroupsList TABLE (Id INT)
	INSERT INTO @ProjectGroupsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIdsLocal)

	-- Convert project owner ids from string to TABLE
	DECLARE @ProjectOwnersList TABLE (Id INT)
	INSERT INTO @ProjectOwnersList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIdsLocal)

	-- Convert salesperson ids from string to TABLE
	DECLARE @SalespersonsList TABLE (Id INT)
	INSERT INTO @SalespersonsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@SalespersonIdsLocal)
	
	
	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	DECLARE @TempProjectResult TABLE(
									ClientId					INT,
									ProjectId					INT PRIMARY KEY,
									Name						NVARCHAR(100),
									PracticeManagerId			INT,
									PracticeId					INT,
									StartDate					DATETIME,
									EndDate						DATETIME,
									ClientName					NVARCHAR(100),
									PracticeName				NVARCHAR(100),
									ProjectStatusId				INT,
									ProjectStatusName			NVARCHAR(25),
									ProjectNumber				NVARCHAR(12),
									GroupId						INT,
									GroupName					NVARCHAR(100),
									SalespersonId				INT,
									SalespersonFirstName		NVARCHAR(40),
									SalespersonLastName			NVARCHAR(40),
									CommissionType				INT,
									PracticeManagerFirstName	NVARCHAR(40),
									PracticeManagerLastName		NVARCHAR(40),
									DirectorId					INT,
									DirectorLastName			NVARCHAR(40),
									DirectorFirstName			NVARCHAR(40),
									[Discount]					DECIMAL (18, 2) 
									)
	INSERT INTO @TempProjectResult
	SELECT  p.ClientId,
			p.ProjectId,
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
			c.PersonId as 'SalespersonId',
			sp.FirstName as SalespersonFirstName,
			sp.LastName as SalespersonLastName,
			c.CommissionType,
			pm.FirstName PracticeManagerFirstName,
			pm.LastName PracticeManagerLastName,
			p.DirectorId,
			p.DirectorLastName,
			p.DirectorFirstName,
			p.[Discount]
	FROM	dbo.v_Project AS p
	INNER JOIN dbo.Person pm ON pm.PersonId = p.PracticeManagerId AND P.IsAdministrative = 0 AND P.ProjectId != 174 
	INNER JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	LEFT JOIN dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN  dbo.Person sp on sp.PersonId = c.PersonId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	WHERE P.ProjectId <> @DefaultProjectId
			AND (c.CommissionType is NULL OR c.CommissionType = 1)
			AND	( (p.StartDate IS NULL AND p.EndDate IS NULL) OR (p.StartDate <= @EndDateLocal AND p.EndDate >= @StartDateLocal))
			AND ( @ClientIdsLocal IS NULL OR p.ClientId IN (SELECT Id from @ClientsList) )
			AND ( @ProjectGroupIdsLocal IS NULL OR p.GroupId IN (SELECT Id from @ProjectGroupsList) )
			AND ( @PracticeIdsLocal IS NULL OR p.PracticeId IN (SELECT Id FROM @PracticesList) OR p.PracticeId IS NULL )
			AND ( @ProjectOwnerIdsLocal IS NULL 
				  OR p.ProjectOwnerId IN (SELECT POL.Id  FROM @ProjectOwnersList POL)
				  OR EXISTS (SELECT 1 FROM dbo.ProjectManagers AS projManagers
								WHERE projManagers.ProjectId = p.ProjectId AND projManagers.ProjectManagerId IN (SELECT POL.Id FROM @ProjectOwnersList POL)
							)
				)
			AND (    @SalespersonIdsLocal IS NULL 
				  OR c.PersonId IN (SELECT Id FROM @SalespersonsList)
			    )
			AND (    ( @ShowProjectedLocal = 1 AND p.ProjectStatusId = 2 )
				  OR ( @ShowActiveLocal = 1 AND p.ProjectStatusId = 3 )
				  OR ( @ShowCompletedLocal = 1 AND p.ProjectStatusId = 4 )
				  OR ( @showInternalLocal = 1 AND p.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimentalLocal = 1 AND p.ProjectStatusId = 5 )
				  OR ( @ShowInactiveLocal = 1 AND p.ProjectStatusId = 1 ) -- Inactive
			)
			AND  (ISNULL(pr.IsCompanyInternal, 0) = 0 AND @ExcludeInternalPracticesLocal  = 1 OR @ExcludeInternalPracticesLocal = 0)
			AND P.IsAllowedToShow = 1
	ORDER BY CASE p.ProjectStatusId
			   WHEN 2 THEN p.StartDate
			   ELSE p.EndDate
			 END

	SELECT * FROM @TempProjectResult

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
		   WHERE cal.Date  BETWEEN @StartDateLocal AND @EndDateLocal

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
				INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN m.StartDate AND m.ProjectedDeliveryDate  AND cal.Date  BETWEEN @StartDateLocal AND @EndDateLocal
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
					   INNER JOIN dbo.Pay AS p ON cal.PersonId = p.Person AND p.StartDate <= cal.Date AND p.EndDate > cal.date AND cal.Date  BETWEEN @StartDateLocal AND @EndDateLocal
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
		FROM @TempProjectResult T
		INNER JOIN dbo.ProjectExpense as pexp ON pexp.ProjectId = T.ProjectId
		INNER JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE c.Date BETWEEN @StartDateLocal AND @EndDateLocal
		GROUP BY pexp.ProjectId, c.MonthStartDate, c.MonthEndDate
	)

	SELECT
		ISNULL(pf.ProjectId,PEM.ProjectId) ProjectId,
		ISNULL(pf.FinancialDate,PEM.FinancialDate) FinancialDate,
		ISNULL(pf.MonthEnd,PEM.MonthEnd) MonthEnd,
		ISNULL(pf.Revenue,0) AS 'Revenue',
		ISNULL(pf.GrossMargin,0)+(ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0))* (1 - ISNULL(pf.Discount,0)/100)  as 'GrossMargin',
		ISNULL(PEM.Expense,0) as 'Expense',
		ISNULL(PEM.Reimbursement,0)  ReimbursedExpense
	FROM ( 
			SELECT f1.ProjectId,
				   cal.MonthStartDate AS FinancialDate,
				   cal.MonthEndDate AS MonthEnd,
				   SUM(f1.PersonMilestoneDailyAmount) AS Revenue,
				   SUM(f1.PersonMilestoneDailyAmount - f1.PersonDiscountDailyAmount -
								(CASE WHEN f1.SLHR >= f1.PayRate + f1.MLFOverheadRate 
									  THEN f1.SLHR ELSE  f1.PayRate + f1.MLFOverheadRate END) 
								*ISNULL(f1.PersonHoursPerDay, 0)) GrossMargin,
				   min(f1.Discount) as Discount
			FROM (
					SELECT f.ProjectId,
							f.PersonMilestoneDailyAmount,
							f.PersonDiscountDailyAmount,
							(ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
							ISNULL(f.PayRate,0) PayRate,
							f.MLFOverheadRate,
							f.PersonHoursPerDay,
							f.Discount,
							f.date
					FROM CTEFinancialsRetroSpective f 
				) as f1
			INNER JOIN dbo.Calendar cal ON f1.Date = cal.Date
			GROUP BY f1.ProjectId,cal.MonthStartDate,cal.MonthEndDate 
		) pf
	FULL JOIN ProjectExpensesMonthly PEM ON PEM.ProjectId = pf.ProjectId AND pf.FinancialDate = PEM.FinancialDate  AND Pf.MonthEnd = PEM.MonthEnd
	
END

