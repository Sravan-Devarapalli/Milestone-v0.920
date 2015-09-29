CREATE PROCEDURE [dbo].[BillingReportByHours]
(
	@StartDate		DATETIME,
	@EndDate		DATETIME,
	@PracticeIds	NVARCHAR(MAX)=NULL,
	@AccountIds		NVARCHAR(MAX),
	@BusinessUnitIds NVARCHAR(MAX),
	@DirectorIds	NVARCHAR(MAX)=NULL,
	@SalesPersonIds NVARCHAR(MAX)=NULL,
	@ProjectManagerIds NVARCHAR(MAX)=NULL,
	@SeniorManagerIds NVARCHAR(MAX)=NULL
)
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @StartDateLocal   DATETIME,
			@EndDateLocal     DATETIME,
			@CurrentMonthStartDate DATETIME,
			@LifeToDateEndDate		DATETIME,
			@Today DATETIME,
			@FutureDate DATETIME,
			@HolidayTimeType INT

    SELECT @StartDateLocal=@StartDate,
		   @EndDateLocal=@EndDate,
		   @Today = dbo.GettingPMTime(GETUTCDATE()),
		   @LifeToDateEndDate= DATEADD(DD,-1,@StartDate)
			
	SELECT @CurrentMonthStartDate = MonthStartDate FROM dbo.Calendar WHERE CONVERT(DATE, Date) = CONVERT(DATE, @Today)
	SELECT @FutureDate = dbo.GetFutureDate(),
		   @HolidayTimeType = dbo.GetHolidayTimeTypeId()
		   
	;WITH ProjectedHours
		AS
		(
			SELECT Pro.ProjectId,
				   ROUND(SUM(CASE WHEN P.IsStrawman = 0 AND PC.Date <= @LifeToDateEndDate THEN dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay) ELSE 0 END),2)  AS ForecastedHours,
				   ROUND(SUM(CASE WHEN P.IsStrawman = 0 AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay) ELSE 0 END),2)  AS ForecastedHoursInRange
		    FROM  dbo.Project Pro
			LEFT JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
			LEFT JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
			LEFT JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
			LEFT JOIN dbo.person AS P ON P.PersonId = MP.PersonId 
			LEFT JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
			WHERE  Pro.projectid != 174 AND (Pro.StartDate <= @EndDateLocal AND @StartDateLocal <= Pro.EndDate) AND
				   (@PracticeIds IS NULL OR Pro.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			       Pro.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
				   Pro.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
				   (@DirectorIds IS NULL OR Pro.ExecutiveInChargeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds))) AND 
				   Pro.StartDate IS NOT NULL AND Pro.EndDate IS NOT NULL
				   AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate AND 
				   (@SalesPersonIds IS NULL OR Pro.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			       (@ProjectManagerIds IS NULL OR 
			         EXISTS (SELECT 1 FROM dbo.ProjectAccess PM WHERE PM.ProjectId = Pro.ProjectId AND PM.ProjectAccessId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds)))) AND
			       (@SeniorManagerIds IS NULL OR Pro.EngagementManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
				   
			GROUP BY  Pro.ProjectId
		),
		TimeEntryHours
		 AS
		 (
			SELECT    Pro.ProjectId,
					  ROUND(SUM(CASE WHEN ( TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000'
								AND TE.ChargeCodeDate <= @LifeToDateEndDate
										) THEN TEH.ActualHours
									ELSE 0
								END),2) AS BillableHours,
					  ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
											AND TE.ChargeCodeDate <= @LifeToDateEndDate
											AND CC.TimeEntrySectionId <> 2
											AND Pro.ProjectNumber != 'P031000'
											AND Pro.IsBusinessDevelopment <> 1 ---- Added this condition as part of PP29 changes by Nick.
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS NonBillableHours,
					  ROUND(SUM(CASE WHEN ( TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000'
											AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
										) THEN TEH.ActualHours
									ELSE 0
								END),2) AS BillableHoursInRange,
					  ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
											AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
											AND CC.TimeEntrySectionId <> 2
											AND Pro.ProjectNumber != 'P031000'
											AND Pro.IsBusinessDevelopment <> 1 ---- Added this condition as part of PP29 changes by Nick.
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS NonBillableHoursInRange						
			FROM      dbo.TimeEntry TE
					INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
					INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = P.PersonId
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																				AND ISNULL(PTSH.EndDate,@FutureDate)
					INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
					INNER JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
			WHERE   (Pro.StartDate <= @EndDateLocal AND @StartDateLocal <= Pro.EndDate) AND
					TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
												@FutureDate)
					AND ( CC.timeTypeId != @HolidayTimeType
							OR ( CC.timeTypeId = @HolidayTimeType
								AND PTSH.PersonStatusId IN (1,5) -- ACTIVE And Terminated Pending
								)
						)
					AND (@PracticeIds IS NULL OR Pro.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			        Pro.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
				    Pro.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
				    (@DirectorIds IS NULL OR Pro.ExecutiveInChargeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds))) AND
					(@SalesPersonIds IS NULL OR Pro.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			  (@ProjectManagerIds IS NULL OR 
			   EXISTS (SELECT 1 FROM dbo.ProjectAccess PM WHERE PM.ProjectId = Pro.ProjectId AND PM.ProjectAccessId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds)))) AND
			   (@SeniorManagerIds IS NULL OR Pro.EngagementManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
			GROUP BY  Pro.ProjectId
		)
	
	SELECT P.ProjectId,
	    P.ClientId,
		C.Name AS ClientName,
		P.ProjectNumber,
		P.Name AS ProjectName,
		Pr.PracticeId,
		Pr.Name AS PracticeName,
		ISNULL(PH.ForecastedHoursInRange,0) AS ForecastedHoursInRange,
		ISNULL(TEH.BillableHoursInRange,0)+ISNULL(TEH.NonBillableHoursInRange,0) AS ActualHoursInRange,
		ISNULL(PH.ForecastedHours,0) AS ForecastedHours,
		ISNULL(TEH.BillableHours,0)+ISNULL(TEH.NonBillableHours,0) AS ActualHours,
		sales.PersonId AS SalesPersonId,
		sales.LastName+', '+ ISNULL(sales.PreferredFirstName,sales.FirstName) as SalesPersonName,
		P.ExecutiveInChargeId AS DirectorId,
	    director.LastName AS DirectorLastName,
		ISNULL(director.PreferredFirstName,director.FirstName) AS DirectorFirstName,
		P.PONumber,
		P.EngagementManagerId AS SeniorManagerId,
		senior.LastName+', '+ISNULL(senior.PreferredFirstName,senior.FirstName) AS SeniorManagerName,
		PM.ProjectAccessId AS ProjectManagerId,
		ISNULL(manager.PreferredFirstName,manager.FirstName) AS ProjectManagerFirstName,
		manager.LastName AS ProjectManagerLastName
	FROM ProjectedHours PH 
	FULL JOIN TimeEntryHours TEH ON TEH.ProjectId = PH.ProjectId
	LEFT JOIN dbo.Project P ON P.ProjectId = ISNULL(PH.ProjectId,TEH.ProjectId)
	INNER JOIN dbo.Client C ON C.ClientId = P.ClientId
	INNER JOIN dbo.Practice Pr ON Pr.PracticeId = P.PracticeId
	INNER JOIN dbo.Person sales ON sales.PersonId = P.SalesPersonId
	LEFT JOIN dbo.Person director ON director.PersonId = P.ExecutiveInChargeId
	LEFT JOIN dbo.Person senior ON senior.PersonId = P.EngagementManagerId
	LEFT JOIN dbo.ProjectAccess PM ON PM.ProjectId = p.ProjectId
	LEFT JOIN dbo.Person manager ON manager.PersonId = PM.ProjectAccessId
	WHERE P.StartDate <= @EndDateLocal AND @StartDateLocal <= P.EndDate
	AND P.ProjectStatusId IN (2,3,7)
	AND p.projectid != 174
	ORDER BY P.ProjectId 
END

