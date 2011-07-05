
CREATE PROCEDURE [dbo].[GetProjectListWithFinancials]
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
	/*union all
	-- All active persons
	select PersonId from Person where PersonStatusId = 1*/
	
	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	DECLARE @TempProjectResult table(
	ClientId					INT,
	ProjectId					INT,
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
	DirectorFirstName			NVARCHAR(40)
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
			p.DirectorFirstName
	FROM	dbo.v_Project AS p
	JOIN dbo.Person pm
	ON pm.PersonId = p.PracticeManagerId
	JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	LEFT JOIN dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN  dbo.Person sp on sp.PersonId = c.PersonId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	WHERE	    (c.CommissionType is NULL OR c.CommissionType = 1)
		    AND (dbo.IsDateRangeWithingTimeInterval(p.StartDate, p.EndDate, @StartDate, @EndDate) = 1 OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR p.ClientId IN (select Id from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT Id from @ProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR p.PracticeId IN (SELECT Id FROM @PracticesList) OR p.PracticeId IS NULL )
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


	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
			+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.PersonId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.ProjectId  IN (SELECT ProjectId FROM @TempProjectResult) 
	),
	ProjectFinancials
	AS
	(
	SELECT f.ProjectId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	     --  (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						--(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
						--	  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					 --   *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       --SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	       --     (f.SLHR) * ISNULL(f.PersonHoursPerDay, 0)) *
	       --    (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
		   min(case when pe.ExpenseSum is null then 0 else pe.ExpenseSum end) as 'Expense',
		   min(case when pe.ReimbursedExpenseSum is null then 0 else pe.ReimbursedExpenseSum end) as 'ReimbursedExpense',
		   min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  LEFT JOIN v_ProjectTotalExpenses as pe on f.ProjectId = pe.ProjectId
	  WHERE  f.Date BETWEEN @StartDate AND @EndDate
	GROUP BY f.ProjectId,YEAR(f.Date), MONTH(f.Date)
	)
	
	SELECT
		pf.ProjectId,
		pf.FinancialDate,
		pf.MonthEnd,
		ISNULL(pf.Revenue,0) AS 'Revenue',
		ISNULL((pf.GrossMargin+(pf.ReimbursedExpense* (1 - pf.Discount/100)) - pf.Expense),0)  as 'GrossMargin'
	FROM ProjectFinancials pf
	JOIN Project p on (p.ProjectId = pf.ProjectId)
	JOIN Practice pr on (pr.PracticeId = p.PracticeId)

