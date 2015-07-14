 CREATE PROCEDURE [dbo].[AccountSummaryByBusinessUnit]
(
	@AccountId	INT,
	@BusinessUnitIds	NVARCHAR(MAX) = NULL,
	@ProjectStatusIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	
	
	DECLARE @StartDateLocal DATETIME ,
		@EndDateLocal DATETIME,
		@HolidayTimeType INT,
		@FutureDate DATETIME,
		@Today DATE

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate), @EndDateLocal = CONVERT(DATE, @EndDate), @HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate(), 
							 @Today = dbo.GettingPMTime(GETUTCDATE())

	DECLARE @BusinessUnitIdsTable TABLE ( Id INT)

	INSERT INTO @BusinessUnitIdsTable(Id)
	SELECT BU.ResultId
	FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds) BU

	DECLARE @ProjectStatusIdsTable TABLE ( Ids INT )
	
	INSERT INTO @ProjectStatusIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectStatusIds)
	
	     ;WITH ProjectedHours
		AS
		(
			SELECT Pro.ProjectId,Pro.GroupId,
				   SUM(CASE WHEN PC.Date <= @Today AND P.IsStrawman = 0 THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay))
									ELSE 0
								END) AS ForecastedHoursUntilToday,
				  ROUND(SUM(CASE WHEN P.IsStrawman = 0 THEN dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay) ELSE 0 END),2)  AS ForecastedHours
		    FROM  dbo.Project Pro
			LEFT JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
			LEFT JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
			LEFT JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
			LEFT JOIN dbo.person AS P ON P.PersonId = MP.PersonId 
			LEFT JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
			WHERE (@BusinessUnitIds IS NULL OR (Pro.GroupId IN (SELECT Id FROM @BusinessUnitIdsTable))) AND Pro.ClientId = @AccountId
				   AND (@ProjectStatusIds IS NULL OR Pro.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusIdsTable))
				   AND Pro.StartDate IS NOT NULL AND Pro.EndDate IS NOT NULL
				   AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal
				   AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate
			GROUP BY  Pro.ProjectId,Pro.GroupId
		),
		 ProjectsDoNotHaveMilestonePersonEntries
		AS
		(
			SELECT DISTINCT p.ProjectId,p.GroupId
			FROM dbo.Project P
			INNER JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId  
			LEFT JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
			LEFT JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
											AND MPE.StartDate <= @EndDateLocal AND @StartDateLocal <= MPE.EndDate
			WHERE 
				(MPE.Id IS NULL OR MP.MilestoneId IS NULL)
				AND (@ProjectStatusIds IS NULL OR P.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusIdsTable))
				AND (@BusinessUnitIds IS NULL OR (P.GroupId IN (SELECT Id FROM @BusinessUnitIdsTable)))
				AND P.ClientId = @AccountId
				AND M.StartDate <= @EndDateLocal AND @StartDateLocal <= M.ProjectedDeliveryDate
		)
		 ,TimeEntryHours
		 AS
		 (
			SELECT    Pro.ProjectId, CC.ProjectGroupId,
						ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
											AND Pro.ProjectNumber != 'P031000'
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS BillableHours ,
						ROUND(SUM(CASE WHEN ( TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000'
								AND TE.ChargeCodeDate <= @Today
										) THEN TEH.ActualHours
									ELSE 0
								END),2) AS BillableHoursUntilToday,
						ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
											AND CC.TimeEntrySectionId <> 2
											AND Pro.ProjectNumber != 'P031000'
											AND Pro.IsBusinessDevelopment <> 1 ---- Added this condition as part of PP29 changes by Nick.
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS NonBillableHours ,
						ROUND(SUM(CASE WHEN (CC.TimeEntrySectionId = 2 OR Pro.IsBusinessDevelopment = 1) -- Added this condition as part of PP29 changes by Nick.
										THEN TEH.ActualHours
										ELSE 0
									END), 2) AS BusinessDevelopmentHours
									
			FROM      dbo.TimeEntry TE
					INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
													AND TE.ChargeCodeDate BETWEEN @StartDateLocal
													AND
													@EndDateLocal
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
					INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = P.PersonId
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																				AND ISNULL(PTSH.EndDate,@FutureDate)
					INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
					INNER JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
			WHERE CC.ClientId = @AccountId
					AND	TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
												@FutureDate)
					AND ( CC.timeTypeId != @HolidayTimeType
							OR ( CC.timeTypeId = @HolidayTimeType
								AND PTSH.PersonStatusId IN (1,5) -- ACTIVE And Terminated Pending
								)
						)
					AND (@BusinessUnitIds IS NULL
						OR CC.ProjectGroupId IN (SELECT *
												FROM @BusinessUnitIdsTable
												)
						)
					AND (@ProjectStatusIds IS NULL OR Pro.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusIdsTable))
			GROUP BY  Pro.ProjectId,CC.ProjectGroupId
		)
	
			SELECT	ISNULL(ISNULL(PH.GroupId, TH.ProjectGroupId),PNP.GroupId) AS GroupId,
				PG.Name AS GroupName, 
				PG.Active, 
				PG.Code AS GroupCode,
				SUM(CASE WHEN P.ProjectStatusId = 4 THEN 1 ELSE 0 END) AS CompletedProjectsCount,
				SUM(CASE WHEN P.ProjectStatusId = 3 THEN 1 ELSE 0 END) AS ActiveProjectsCount,
				SUM(CASE WHEN P.ProjectStatusId IS NULL THEN 0 ELSE 1 END) AS ProjectsCount,
			    CAST((ROUND(ISNULL(SUM(PH.ForecastedHours),0), 2)) AS float) as ForecastedHours,
				CAST((ROUND(ISNULL(SUM(PH.ForecastedHoursUntilToday),0), 2)) AS float) as ForecastedHoursUntilToday,
				ROUND(ISNULL(SUM(TH.BillableHours),0), 2) as BillableHours,
				ROUND(ISNULL(SUM(TH.BillableHoursUntilToday),0), 2) as BillableHoursUntilToday,
				ROUND(ISNULL(SUM(TH.NonBillableHours),0), 2) as NonBillableHours,
				ROUND(ISNULL(SUM(TH.BusinessDevelopmentHours),0), 2) as BusinessDevelopmentHours
		FROM TimeEntryHours TH  
		FULL JOIN ProjectedHours PH ON PH.ProjectId = TH.ProjectId
		FULL JOIN ProjectsDoNotHaveMilestonePersonEntries PNP ON PNP.ProjectId = PH.ProjectId OR PNP.ProjectId = TH.ProjectId 
		LEFT JOIN ProjectGroup PG ON PG.GroupId = ISNULL(ISNULL(PH.GroupId, TH.ProjectGroupId),PNP.GroupId)
		LEFT JOIN dbo.Project P ON P.ProjectId =  ISNULL(ISNULL(PH.ProjectId, TH.ProjectId),PNP.ProjectId)
		WHERE P.ProjectNumber != 'P031000'	
		GROUP BY ISNULL(ISNULL(PH.GroupId, TH.ProjectGroupId),PNP.GroupId),PG.Name,PG.Active,PG.Code
	

		SELECT COUNT(DISTINCT p.PersonId) as PersonsCount,
				c.Name AS ClientName,
				c.Code AS ClientCode,
				c.ClientId AS ClientId
		FROM dbo.Project Pro
				INNER JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
				INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
				INNER JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
				INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId
				INNER JOIN dbo.Client C ON C.ClientId = Pro.ClientId
		WHERE C.ClientId = @AccountId
				AND MPE.StartDate <= @EndDateLocal AND @StartDateLocal <= MPE.EndDate
				AND (@ProjectStatusIds IS NULL OR Pro.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusIdsTable))
				AND (@BusinessUnitIds IS NULL
					OR Pro.GroupId IN (SELECT *
											FROM @BusinessUnitIdsTable
											)
					)
		GROUP BY C.ClientId, C.Name, C.Code
		

END

