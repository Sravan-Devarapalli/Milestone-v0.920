CREATE PROCEDURE dbo.PersonStatsByDateRange
(
	@StartDate           DATETIME,
	@EndDate             DATETIME,
	@SalespersonId       INT = NULL,
	@PracticeManagerId   INT = NULL,
	@ShowProjected		 BIT = NULL,
	@ShowCompleted		 BIT = NULL,
    @ShowActive			 BIT = NULL,
	@ShowExperimental	 BIT = NULL,
	@ShowInternal		 BIT = NULL,
	@ShowInactive		 BIT = NULL
)
AS
	SET NOCOUNT ON	

	SELECT ISNULL(SUM(
			CASE	
				WHEN (p.ProjectStatusId = 2 AND @ShowProjected = 'TRUE') OR @ShowProjected IS NULL THEN f.PersonMilestoneDailyAmount 
				WHEN (p.ProjectStatusId = 4 AND @ShowCompleted = 'TRUE') OR @ShowCompleted IS NULL THEN f.PersonMilestoneDailyAmount 
				WHEN (p.ProjectStatusId = 3 AND @ShowActive = 'TRUE') OR @ShowActive IS NULL THEN f.PersonMilestoneDailyAmount 
				WHEN (p.ProjectStatusId = 5 AND @ShowExperimental = 'TRUE') OR @ShowExperimental IS NULL THEN f.PersonMilestoneDailyAmount 
				WHEN (p.ProjectStatusId = 6 AND @ShowInternal = 'TRUE') OR @ShowInternal IS NULL THEN f.PersonMilestoneDailyAmount
				WHEN (p.ProjectStatusId = 1 AND @ShowInactive = 'TRUE') OR @ShowInactive IS NULL THEN f.PersonMilestoneDailyAmount 
				ELSE 0 
			END), 0) AS Revenue,
			dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), 1) AS Date,
			dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), NULL) as EndDate,
	       ISNULL(SUM(CASE WHEN pr.DefaultPractice <> 1 /* Non in offshore practice*/ THEN f.PersonHoursPerDay ELSE 0 END), 0) /
	       (SELECT COUNT(*) * 8
	          FROM dbo.Calendar AS c
	         WHERE c.Date BETWEEN dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), 1) AND dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), NULL)
	           AND c.DayOff = 0) AS VirtualConsultants,
	       EmployeesNumber = dbo.GetEmployeeNumber(dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), 1), dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), NULL)),
		   ConsultantsNumber = dbo.GetCounsultantsNumber(dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), 1), dbo.MakeDate(YEAR(cal.Date), MONTH(cal.Date), NULL))
	  FROM dbo.Calendar AS cal
	       LEFT JOIN dbo.v_FinancialsRetrospective AS f
	          ON f.Date BETWEEN @StartDate AND @EndDate AND cal.Date = f.Date
	       LEFT JOIN dbo.Person AS pr ON f.PersonId = pr.PersonId
	       LEFT JOIN dbo.Project AS p ON f.ProjectId = p.ProjectId
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
       AND (   (@SalespersonId IS NULL AND @PracticeManagerId IS NULL)
	        OR EXISTS (SELECT 1
	                      FROM dbo.v_PersonProjectCommission AS c
	                     WHERE c.ProjectId = p.ProjectId AND c.PersonId = @SalespersonId AND c.CommissionType = 1
	                   UNION ALL
	                   SELECT 1
	                     FROM dbo.v_PersonProjectCommission AS c
	                    WHERE c.ProjectId = p.ProjectId AND c.PersonId = @PracticeManagerId AND c.CommissionType = 2 
	                   )
	       )
	GROUP BY YEAR(cal.Date), MONTH(cal.Date)
	ORDER BY YEAR(cal.Date), MONTH(cal.Date)

