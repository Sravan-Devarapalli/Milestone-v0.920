﻿CREATE PROCEDURE [dbo].[BillingReportByHours]
(
	@StartDate		DATETIME,
	@EndDate		DATETIME,
	@PracticeIds	NVARCHAR(MAX)=NULL,
	@AccountIds		NVARCHAR(MAX),
	@BusinessUnitIds NVARCHAR(MAX),
	@DirectorIds	NVARCHAR(MAX),
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
			@Today DATETIME,
			@FutureDate DATETIME,
			@HolidayTimeType INT


    SELECT @StartDateLocal=@StartDate,
		   @EndDateLocal=@EndDate,
		   @Today = dbo.GettingPMTime(GETUTCDATE())
			
	SELECT @CurrentMonthStartDate = MonthStartDate FROM dbo.Calendar WHERE CONVERT(DATE, Date) = CONVERT(DATE, @Today)
	SELECT @FutureDate = dbo.GetFutureDate(),
		   @HolidayTimeType = dbo.GetHolidayTimeTypeId()
		   
;WITH ActualTimeEntries
AS 
(
	SELECT CC.ProjectId,
			TE.PersonId,
			TE.ChargeCodeDate,
			SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END) BillableHOursPerDay,
			P.IsHourlyAmount
	FROM TimeEntry TE
	JOIN TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
	JOIN ChargeCode CC on CC.Id = TE.ChargeCodeId AND cc.projectid != 174
	JOIN (
			SELECT Pro.ProjectId,CAST(CASE WHEN SUM(CAST(m.IsHourlyAmount as INT)) > 0 THEN 1 ELSE 0 END AS BIT) AS IsHourlyAmount
			FROM Project Pro 
				LEFT JOIN Milestone m ON m.ProjectId = Pro.ProjectId 
			WHERE Pro.IsAllowedToShow = 1 AND Pro.ProjectStatusId IN (2,3,7) AND
			  (@PracticeIds IS NULL OR Pro.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			  Pro.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
			  Pro.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
			  Pro.DirectorId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds)) AND
			   (@SalesPersonIds IS NULL OR Pro.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			  (@ProjectManagerIds IS NULL OR 
			   EXISTS (SELECT 1 FROM dbo.ProjectManagers PM WHERE PM.ProjectId = Pro.ProjectId AND PM.ProjectManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@ProjectManagerIds)))) AND
			   (@SeniorManagerIds IS NULL OR Pro.SeniorManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
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
			INNER JOIN dbo.[Milestone] AS m ON P.ProjectId=m.ProjectId AND p.IsAllowedToShow = 1 AND p.projectid != 174
			INNER JOIN dbo.MilestonePerson AS mp ON m.[MilestoneId] = mp.[MilestoneId]
			INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
			INNER JOIN dbo.PersonCalendarAuto AS cal ON cal.Date BETWEEN mpe.Startdate AND mpe.EndDate AND cal.PersonId = mp.PersonId 
			WHERE P.ProjectStatusId IN (2,3,7) AND
			  (@PracticeIds IS NULL OR P.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			  P.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
			  P.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
			  P.DirectorId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds)) AND
			   (@SalesPersonIds IS NULL OR P.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			  (@ProjectManagerIds IS NULL OR 
			   EXISTS (SELECT 1 FROM dbo.ProjectManagers PM WHERE PM.ProjectId = P.ProjectId AND PM.ProjectManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@ProjectManagerIds)))) AND
			   (@SeniorManagerIds IS NULL OR P.SeniorManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
		    GROUP BY  m.ProjectId,m.[MilestoneId],mp.PersonId,cal.Date,m.IsHourlyAmount ,m.IsDefault,MPE.Id,MPE.Amount
),
CTE 
AS 
(
	SELECT s.Date, s.MilestoneId, SUM(HoursPerDay) AS HoursPerDay
	FROM MileStoneEntries AS s
	WHERE s.IsHourlyAmount = 0
	GROUP BY s.Date, s.MilestoneId
),
MilestoneRevenueRetrospective
AS
(
	SELECT -- Milestones with a fixed amount
			m.MilestoneId,
			cal.Date,
			m.IsHourlyAmount,
			ISNULL((m.Amount/ NULLIF(MTHours.TotalHours,0))* d.HoursPerDay,0) AS MilestoneDailyAmount /* ((Milestone fixed amount/Milestone Total  Hours)* Milestone Total  Hours per day)  */,
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
		UNION ALL
	SELECT -- Milestones with a hourly amount
			mp.MilestoneId,
			mp.Date,
			mp.IsHourlyAmount,
			ISNULL(SUM(mp.Amount * mp.HoursPerDay), 0) AS MilestoneDailyAmount,
			SUM(mp.HoursPerDay) AS HoursPerDay/* Milestone Total  Hours per day*/
		FROM MileStoneEntries mp
			INNER JOIN dbo.Project AS p ON mp.ProjectId = p.ProjectId AND mp.IsHourlyAmount = 1
	GROUP BY mp.MilestoneId, mp.Date, mp.IsHourlyAmount
),
cteFinancialsRetrospectiveActualHours
as
(
	SELECT	pro.ProjectId,
		Per.PersonId,
		c.Date,
		AE.BillableHOursPerDay,
		ISNULL(ME.IsHourlyAmount,AE.IsHourlyAmount) AS IsHourlyAmount,
		ME.ActualHoursPerDay,
		CASE
	           WHEN ME.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ME.PersonMilestoneDailyAmount
	           ELSE ISNULL(r.MilestoneDailyAmount * ME.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END AS PersonMilestoneDailyAmount,--Person Level Daily Amount
		 CASE
	           WHEN ME.IsHourlyAmount = 1
	           THEN ME.Amount
	           WHEN ME.IsHourlyAmount = 0 AND r.HoursPerDay = 0
	           THEN 0
	           ELSE r.MilestoneDailyAmount / r.HoursPerDay
		   END AS BillRate
FROM ActualTimeEntries AS AE --ActualEntriesByPerson
		FULL JOIN MileStoneEntries AS ME ON ME.ProjectId = AE.ProjectId AND AE.PersonId = ME.PersonId AND ME.Date = AE.ChargeCodeDate 
		INNER JOIN dbo.Person Per ON per.PersonId = ISNULL(ME.PersonId,AE.PersonId)
		INNER JOIN dbo.Project Pro ON Pro.ProjectId = ISNULL(ME.ProjectId,AE.ProjectId) 
		INNER JOIN dbo.Calendar C ON c.Date = ISNULL(ME.Date,AE.ChargeCodeDate)		
		LEFT JOIN MilestoneRevenueRetrospective AS r ON ME.MilestoneId = r.MilestoneId AND c.Date = r.Date
	GROUP BY pro.ProjectId,Per.PersonId,c.Date,C.DaysInYear,AE.BillableHOursPerDay,ME.IsHourlyAmount,ME.HoursPerDay,ME.PersonMilestoneDailyAmount,
			r.HoursPerDay,r.MilestoneDailyAmount,ME.Amount,ME.Id,ME.ActualHoursPerDay,AE.IsHourlyAmount
),
	 FinancialsRetro AS 
	(
		SELECT f.ProjectId,
			   f.Date, 
			   f.PersonMilestoneDailyAmount,
			   f.PersonId,
			   f.IsHourlyAmount,
			   f.BillableHOursPerDay,
			   f.ActualHoursPerDay,
			   f.BillRate
		FROM cteFinancialsRetrospectiveActualHours f
		INNER JOIN dbo.Project P ON P.ProjectId = f.ProjectId
		WHERE (P.StartDate <= @EndDateLocal and @StartDateLocal<= P.EndDate)
	),
	ActualAndProjectedValuesDaily
	AS
	(
	SELECT	f.ProjectId,
			f.PersonId,
			f.Date,
			SUM(CASE WHEN f.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN f.PersonMilestoneDailyAmount ELSE 0 END) AS ProjectedRevenueperDay,
			SUM(CASE WHEN f.IsHourlyAmount = 0 AND f.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN f.PersonMilestoneDailyAmount ELSE 0 END ) AS FixedActualRevenuePerDayInRange,
			(ISNULL(SUM(CASE WHEN f.IsHourlyAmount = 1 AND f.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN f.BillRate* f.ActualHoursPerDay ELSE 0 END),0) / ISNULL(NULLIF(SUM(CASE WHEN f.IsHourlyAmount = 1 AND f.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN f.ActualHoursPerDay ELSE 0 END),0),1)) * MAX(CASE WHEN f.IsHourlyAmount = 1 AND f.Date BETWEEN @StartDateLocal AND @EndDateLocal THEN  f.BillableHOursPerDay ELSE 0 END) HourlyActualRevenuePerDayInRange
	FROM FinancialsRetro AS f
	GROUP BY f.ProjectId, f.PersonId, f.Date
	),
	ActualAndProjectedValuesMonthly  AS
	(SELECT CT.ProjectId, 
			SUM(ISNULL(CT.ProjectedRevenueperDay, 0)) AS ProjectedRevenueInRange,
			SUM(CASE WHEN CT.Date < @CurrentMonthStartDate THEN ISNULL(CT.HourlyActualRevenuePerDayInRange, 0) + ISNULL(CT.FixedActualRevenuePerDayInRange, 0) ELSE 0 END) AS ActualRevenueInRange
	FROM ActualAndProjectedValuesDaily CT
	GROUP BY CT.ProjectId
	),
	  ProjectedHours
		AS
		(
			SELECT Pro.ProjectId,
				   ROUND(SUM(CASE WHEN P.IsStrawman = 0 THEN dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay) ELSE 0 END),2)  AS ForecastedHours
		    FROM  dbo.Project Pro
			LEFT JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
			LEFT JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
			LEFT JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
			LEFT JOIN dbo.person AS P ON P.PersonId = MP.PersonId 
			LEFT JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
			WHERE  (@PracticeIds IS NULL OR Pro.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			       Pro.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
				   Pro.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
				   Pro.DirectorId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds)) AND 
				   Pro.StartDate IS NOT NULL AND Pro.EndDate IS NOT NULL
				   AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate AND 
				   (@SalesPersonIds IS NULL OR Pro.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			       (@ProjectManagerIds IS NULL OR 
			         EXISTS (SELECT 1 FROM dbo.ProjectManagers PM WHERE PM.ProjectId = Pro.ProjectId AND PM.ProjectManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds)))) AND
			       (@SeniorManagerIds IS NULL OR Pro.SeniorManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
				   
			GROUP BY  Pro.ProjectId
		),
		TimeEntryHours
		 AS
		 (
			SELECT    Pro.ProjectId,
					  ROUND(SUM(CASE WHEN ( TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000'
								AND TE.ChargeCodeDate <= @CurrentMonthStartDate
										) THEN TEH.ActualHours
									ELSE 0
								END),2) AS BillableHours,
					  ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
											AND CC.TimeEntrySectionId <> 2
											AND Pro.ProjectNumber != 'P031000'
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS NonBillableHours			
			FROM      dbo.TimeEntry TE
					INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
					INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = P.PersonId
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																				AND ISNULL(PTSH.EndDate,@FutureDate)
					INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
					INNER JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
			WHERE   TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
												@FutureDate)
					AND ( CC.timeTypeId != @HolidayTimeType
							OR ( CC.timeTypeId = @HolidayTimeType
								AND PTSH.PersonStatusId IN (1,5) -- ACTIVE And Terminated Pending
								)
						)
					AND (@PracticeIds IS NULL OR Pro.PracticeId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))) AND
			        Pro.ClientId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@AccountIds)) AND
				    Pro.GroupId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)) AND
				    Pro.DirectorId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@DirectorIds)) AND
					(@SalesPersonIds IS NULL OR Pro.SalesPersonId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds))) AND
			  (@ProjectManagerIds IS NULL OR 
			   EXISTS (SELECT 1 FROM dbo.ProjectManagers PM WHERE PM.ProjectId = Pro.ProjectId AND PM.ProjectManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SalesPersonIds)))) AND
			   (@SeniorManagerIds IS NULL OR Pro.SeniorManagerId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@SeniorManagerIds)))
			GROUP BY  Pro.ProjectId
		)
	
	SELECT APV.ProjectId,
	    P.ClientId,
		C.Name AS ClientName,
		P.ProjectNumber,
		P.Name AS ProjectName,
		Pr.PracticeId,
		Pr.Name AS PracticeName,
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedRevenueInRange,0)) AS 'Revenue',
		CONVERT(DECIMAL(18,6), ISNULL(APV.ActualRevenueInRange,0)) ActualRevenueInRange,
		ISNULL(PH.ForecastedHours,0) AS ForecastedHours,
		ISNULL(TEH.BillableHours,0)+ISNULL(TEH.NonBillableHours,0) AS ActualHours,
		sales.PersonId AS SalesPersonId,
		sales.LastName+', '+ sales.FirstName as SalesPersonName,
		P.DirectorId,
	    director.LastName AS DirectorLastName,
		director.FirstName AS DirectorFirstName,
		P.PONumber,
		P.SeniorManagerId,
		senior.LastName+', '+senior.FirstName AS SeniorManagerName,
		PM.ProjectManagerId,
		manager.FirstName AS ProjectManagerFirstName,
		manager.LastName AS ProjectManagerLastName
	FROM ActualAndProjectedValuesMonthly APV
	INNER JOIN dbo.Project P ON P.ProjectId = APV.ProjectId
	INNER JOIN dbo.Client C ON C.ClientId = P.ClientId
	INNER JOIN dbo.Practice Pr ON Pr.PracticeId = P.PracticeId
	INNER JOIN dbo.Person sales ON sales.PersonId = P.SalesPersonId
	LEFT JOIN dbo.Person director ON director.PersonId = P.DirectorId
	LEFT JOIN dbo.Person senior ON senior.PersonId = P.SeniorManagerId
	LEFT JOIN ProjectedHours PH ON PH.ProjectId = P.ProjectId
	LEFT JOIN TimeEntryHours TEH ON TEH.ProjectId = P.ProjectId
		LEFT JOIN dbo.ProjectManagers PM ON PM.ProjectId = p.ProjectId
	LEFT JOIN dbo.Person manager ON manager.PersonId = PM.ProjectManagerId
	ORDER BY ProjectId 
END
