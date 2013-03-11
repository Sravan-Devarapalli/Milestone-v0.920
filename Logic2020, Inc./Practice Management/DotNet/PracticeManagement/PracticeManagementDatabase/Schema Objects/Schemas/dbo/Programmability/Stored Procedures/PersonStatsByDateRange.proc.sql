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
	@ShowInactive		 BIT = NULL,
	@UseActuals			 BIT = 0
)
AS
	SET NOCOUNT ON	

	DECLARE 
	@StartDateLocal      DATETIME,
	@EndDateLocal             DATETIME,
	@SalespersonIdLocal       INT = NULL,
	@PracticeManagerIdLocal   INT = NULL,
	@ShowProjectedLocal		 BIT = NULL,
	@ShowCompletedLocal		 BIT = NULL,
    @ShowActiveLocal			 BIT = NULL,
	@ShowExperimentalLocal	 BIT = NULL,
	@ShowInternalLocal		 BIT = NULL,
	@ShowInactiveLocal		 BIT = NULL,
	@UseActualsLocal			 BIT = 0

	SELECT 	@StartDateLocal      =@StartDate,
	@EndDateLocal             =@EndDate,
	@SalespersonIdLocal       =@SalespersonId,
	@PracticeManagerIdLocal  =@PracticeManagerId,
	@ShowProjectedLocal		 =@ShowProjected,
	@ShowCompletedLocal		 =@ShowCompleted,
    @ShowActiveLocal		=@ShowActive,
	@ShowExperimentalLocal	=@ShowExperimental,
	@ShowInternalLocal		=@ShowInternal,
	@ShowInactiveLocal		=@ShowInactive,
	@UseActualsLocal		=@UseActuals

	DECLARE @Today DATETIME, @CurrentMonthStartDate DATETIME
	SELECT @Today = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))
	SELECT @CurrentMonthStartDate = C.MonthStartDate
	FROM dbo.Calendar C
	WHERE C.Date = @Today

	;WITH ProjectedValues
	AS
	(
		SELECT	FR.ProjectId,
				FR.PersonId,
				FR.Date,
				SUM(FR.PersonMilestoneDailyAmount) PersonMilestoneDailyAmount,
				MIN(CONVERT(INT, FR.IsHourlyAmount)) as IsHourlyAmount,
				SUM(FR.PersonHoursPerDay) PersonHoursPerDay
		FROM dbo.v_FinancialsRetrospective AS FR 
		WHERE FR.Date BETWEEN @StartDateLocal AND @EndDateLocal
		GROUP BY FR.ProjectId,FR.PersonId,FR.Date
	)
	SELECT  cal.MonthStartDate AS Date,
			cal.MonthEndDate as EndDate,
			ISNULL(SUM
						(
							CASE WHEN cal.MonthStartDate < @CurrentMonthStartDate  AND @UseActualsLocal = 1 AND CT.IsHourlyAmount = 1
							THEN	(
										CASE	
											WHEN (p.ProjectStatusId = 2 AND @ShowProjectedLocal = 'TRUE') OR @ShowProjectedLocal IS NULL THEN CT.ActualDayRevenue
											WHEN (p.ProjectStatusId = 4 AND @ShowCompletedLocal = 'TRUE') OR @ShowCompletedLocal IS NULL THEN CT.ActualDayRevenue
											WHEN (p.ProjectStatusId = 3 AND @ShowActiveLocal = 'TRUE') OR @ShowActiveLocal IS NULL THEN CT.ActualDayRevenue
											WHEN (p.ProjectStatusId = 5 AND @ShowExperimentalLocal = 'TRUE') OR @ShowExperimentalLocal IS NULL THEN CT.ActualDayRevenue
											WHEN (p.ProjectStatusId = 6 AND @ShowInternalLocal = 'TRUE') OR @ShowInternalLocal IS NULL THEN CT.ActualDayRevenue
											WHEN (p.ProjectStatusId = 1 AND @ShowInactiveLocal = 'TRUE') OR @ShowInactiveLocal IS NULL THEN CT.ActualDayRevenue
											ELSE 0 
										END
									)
							ELSE
									(
										CASE	
											WHEN (p.ProjectStatusId = 2 AND @ShowProjectedLocal = 'TRUE') OR @ShowProjectedLocal IS NULL THEN CT.PersonMilestoneDailyAmount 
											WHEN (p.ProjectStatusId = 4 AND @ShowCompletedLocal = 'TRUE') OR @ShowCompletedLocal IS NULL THEN CT.PersonMilestoneDailyAmount 
											WHEN (p.ProjectStatusId = 3 AND @ShowActiveLocal = 'TRUE') OR @ShowActiveLocal IS NULL THEN CT.PersonMilestoneDailyAmount 
											WHEN (p.ProjectStatusId = 5 AND @ShowExperimentalLocal = 'TRUE') OR @ShowExperimentalLocal IS NULL THEN CT.PersonMilestoneDailyAmount 
											WHEN (p.ProjectStatusId = 6 AND @ShowInternalLocal = 'TRUE') OR @ShowInternalLocal IS NULL THEN CT.PersonMilestoneDailyAmount
											WHEN (p.ProjectStatusId = 1 AND @ShowInactiveLocal = 'TRUE') OR @ShowInactiveLocal IS NULL THEN CT.PersonMilestoneDailyAmount 
											ELSE 0 
										END
									)
							END
				), 0) AS Revenue,
	       ISNULL(SUM(CASE WHEN pr.DefaultPractice <> 1 /* Non in offshore practice*/ THEN CT.PersonHoursPerDay ELSE 0 END), 0) /
	       (SELECT COUNT(*) * 8
				FROM dbo.Calendar AS c
	         WHERE c.Date BETWEEN cal.MonthStartDate AND cal.MonthEndDate
	           AND c.DayOff = 0) AS VirtualConsultants,
	       EmployeesNumber = dbo.GetEmployeeNumber(cal.MonthStartDate, cal.MonthEndDate),
		   ConsultantsNumber = dbo.GetCounsultantsNumber(cal.MonthStartDate, cal.MonthEndDate)
	  FROM (
			SELECT	ISNULL(FR.ProjectId,FRA.ProjectId) AS ProjectId,
					ISNULL(FR.PersonId,FRA.PersonId) AS PersonId,
					ISNULL(FR.Date,FRA.date) AS Date,
					FR.PersonMilestoneDailyAmount,
					FR.IsHourlyAmount,
					FR.PersonHoursPerDay,
					CONVERT(DECIMAL,ISNULL(FRA.BillRate * FRA.BillableHOursPerDay,0)) AS ActualDayRevenue
			FROM ProjectedValues AS FR 
			LEFT JOIN v_FinancialsRetrospectiveActualHours FRA ON @UseActualsLocal = 1 AND FR.Date = FRA.Date AND FRA.ProjectId = FR.ProjectId AND FR.PersonId = FRA.PersonId
			WHERE ISNULL(FR.DATE,FRA.Date) BETWEEN @StartDateLocal AND @EndDateLocal
			UNION 
			SELECT	ISNULL(FR.ProjectId,FRA.ProjectId) AS ProjectId,
					ISNULL(FR.PersonId,FRA.PersonId) AS PersonId,
					ISNULL(FR.Date,FRA.date) AS Date,
					FR.PersonMilestoneDailyAmount,
					FR.IsHourlyAmount,
					FR.PersonHoursPerDay,
					CONVERT(DECIMAL,ISNULL(FRA.BillRate * FRA.BillableHOursPerDay,0)) AS ActualDayRevenue
			FROM ProjectedValues AS FR 
			RIGHT JOIN v_FinancialsRetrospectiveActualHours FRA ON @UseActualsLocal = 1 AND FR.Date = FRA.Date AND FRA.ProjectId = FR.ProjectId AND FR.PersonId = FRA.PersonId
			WHERE ISNULL(FR.DATE,FRA.Date) BETWEEN @StartDateLocal AND @EndDateLocal
			) CT
			INNER JOIN dbo.Calendar AS cal ON cal.Date = CT.Date
			INNER JOIN dbo.Person pr ON pr.PersonId = CT.PersonId
			INNER JOIN dbo.Project P ON P.ProjectId = CT.ProjectId
	 WHERE ((@SalespersonIdLocal IS NULL AND @PracticeManagerIdLocal IS NULL)
	        OR EXISTS (SELECT 1
	                      FROM dbo.v_PersonProjectCommission AS c
	                     WHERE c.ProjectId = p.ProjectId AND c.PersonId = @SalespersonIdLocal AND c.CommissionType = 1
	                   UNION ALL
	                   SELECT 1
	                     FROM dbo.v_PersonProjectCommission AS c
	                    WHERE c.ProjectId = p.ProjectId AND c.PersonId = @PracticeManagerIdLocal AND c.CommissionType = 2 
	                   )
	       )
	GROUP BY cal.MonthStartDate, cal.MonthEndDate
	ORDER BY cal.MonthStartDate

