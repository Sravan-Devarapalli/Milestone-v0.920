﻿
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
BEGIN
	SET NOCOUNT ON ;

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

	DECLARE @TempProjectResult TABLE(
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
			AND ( @ClientIds IS NULL OR p.ClientId IN (SELECT Id from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT Id from @ProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR p.PracticeId IN (SELECT Id FROM @PracticesList) OR p.PracticeId IS NULL )
			AND ( @ProjectOwnerIds IS NULL 
				  OR EXISTS (SELECT 1 FROM dbo.ProjectManagers AS projManagers
							JOIN @ProjectOwnersList POL ON POL.Id = projManagers.ProjectManagerId
								WHERE projManagers.ProjectId = p.ProjectId
							)
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
		   min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  
	  WHERE  f.Date BETWEEN @StartDate AND @EndDate
	 GROUP BY f.ProjectId,YEAR(f.Date), MONTH(f.Date)
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

	SELECT
		ISNULL(pf.ProjectId,PEM.ProjectId) ProjectId,
		ISNULL(pf.FinancialDate,PEM.FinancialDate) FinancialDate,
		ISNULL(pf.MonthEnd,PEM.MonthEnd) MonthEnd,
		ISNULL(pf.Revenue,0)+ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0) AS 'Revenue',
		ISNULL(pf.GrossMargin,0)+(ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0))* (1 - ISNULL(pf.Discount,0)/100)  as 'GrossMargin',
		ISNULL(PEM.Expense,0) as 'Expense',
		ISNULL(PEM.Reimbursement,0)  ReimbursedExpense
	FROM ProjectFinancials pf
	JOIN Project p on (p.ProjectId = pf.ProjectId)
	JOIN Practice pr on (pr.PracticeId = p.PracticeId)
	FULL JOIN ProjectExpensesMonthly PEM 
	ON PEM.ProjectId = pf.ProjectId AND pf.FinancialDate = PEM.FinancialDate  AND Pf.MonthEnd = PEM.MonthEnd
	
END
