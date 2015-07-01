CREATE PROCEDURE [dbo].[AccountSummaryByProject]
(
	@AccountId	INT,
	@BusinessUnitIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@ProjectStatusIds	NVARCHAR(MAX) = NULL,
	@ProjectBillingTypes	NVARCHAR(MAX) = NULL
)
AS
BEGIN
	

		DECLARE @StartDateLocal DATETIME ,
		@EndDateLocal DATETIME ,
		@Today DATE ,
		@HolidayTimeType INT,
		@FutureDate DATETIME,
		@AccountIdLocal NVARCHAR(MAX),
		@BusinessUnitIdsLocal	NVARCHAR(MAX),
		@ProjectStatusIdsLocal	NVARCHAR(MAX),
		@ProjectBillingTypesLocal	NVARCHAR(MAX)

	DECLARE @ProjectBillingTypesTable TABLE( BillingType NVARCHAR(100) )
	DECLARE @BusinessUnitIdsTable TABLE ( Ids INT )
	DECLARE @ProjectStatusIdsTable TABLE ( Ids INT )

	SET @AccountIdLocal = @AccountId
	SET @BusinessUnitIdsLocal = @BusinessUnitIds
	SET @ProjectStatusIdsLocal=@ProjectStatusIds
	SET @ProjectBillingTypesLocal = @ProjectBillingTypes

	INSERT INTO @ProjectBillingTypesTable(BillingType)
	SELECT ResultString
	FROM [dbo].[ConvertXmlStringInToStringTable](@ProjectBillingTypesLocal)

	INSERT INTO @BusinessUnitIdsTable(Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@BusinessUnitIdsLocal)

	INSERT INTO @ProjectStatusIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectStatusIdsLocal)


	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)
		 , @Today = dbo.GettingPMTime(GETUTCDATE())
		 , @HolidayTimeType = dbo.GetHolidayTimeTypeId()
		 , @FutureDate = dbo.GetFutureDate()

	;WITH PersonForeCastedHoursForBillRate
		AS
		(	
			SELECT   MP.PersonId ,
					 Pro.ClientId,
					 Pro.ProjectId,
				     AVG(MPE.Amount) AS BillRate,
					 PC.Date,
				     SUM(CASE WHEN PC.Date <= @Today AND P.IsStrawman = 0 THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay))
									ELSE 0
								END) AS ForecastedHoursUntilToday
				,SUM(CASE WHEN P.IsStrawman = 0 THEN dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay) ELSE 0 END)  AS ForecastedHours
				, MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue
				, MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
			FROM	dbo.Project Pro
				LEFT JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
				LEFT JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
				LEFT JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
				LEFT JOIN dbo.person AS P ON P.PersonId = MP.PersonId 
				LEFT JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId 
						AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate 
						AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal 
						  
		WHERE (@ProjectStatusIds IS NULL
					 OR Pro.ProjectStatusId IN (SELECT Ids
												FROM @ProjectStatusIdsTable))
			  AND Pro.ClientId = @AccountIdLocal
			   AND (@BusinessUnitIds IS NULL
					OR Pro.GroupId IN (SELECT Ids
											FROM @BusinessUnitIdsTable )
				)
			  AND Pro.StartDate IS NOT NULL AND Pro.EndDate IS NOT NULL
			  AND M.StartDate <= @EndDateLocal AND @StartDateLocal <= M.ProjectedDeliveryDate
					
			GROUP BY MP.PersonId,Pro.ProjectId,Pro.ClientId,PC.Date 
		),
		TimeEntryPersonsForBillRate 
		AS
		(
			SELECT TE.PersonId,PRO.ProjectId,TE.ChargeCodeDate,CC.ClientId
					, CC.ProjectGroupId AS GroupId
					, CC.TimeEntrySectionId,
				 ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [BillableHours]
			  , ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 0 OR PRO.ProjectNumber = 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [NonBillableHours]
			  , ROUND(SUM(CASE
					WHEN (TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' AND TE.ChargeCodeDate <= @Today) THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS BillableHoursUntilToday
			FROM TimeEntry TE
			INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
														AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
			INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
													AND ISNULL(PTSH.EndDate,@FutureDate)
			INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId 
										AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
										AND ( 
												CC.timeTypeId != @HolidayTimeType
												OR ( 
														CC.timeTypeId = @HolidayTimeType
														AND PTSH.PersonStatusId IN (1,5)
													)
											)
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
			WHERE  CC.ClientId = @AccountIdLocal
					AND (@BusinessUnitIds IS NULL
							OR CC.ProjectGroupId IN (SELECT Ids
													FROM @BusinessUnitIdsTable )
						)
					 AND (@ProjectStatusIds IS NULL
							 OR PRO.ProjectStatusId IN (SELECT Ids
														FROM @ProjectStatusIdsTable )
						)
			GROUP BY TE.PersonId,
					PRO.ProjectId,
					TE.ChargeCodeDate,
					CC.ClientId,
					CC.ProjectGroupId,
					CC.TimeEntrySectionId
		),
	EstimatedBillingsByProject
		AS 
		(
		  SELECT Proj.ProjectId,
		         ROUND(SUM(Proj.BillRate*Proj.BillableHours),2) AS EstBillings
		  From
			(SELECT	PFR.ProjectId,
					PFR.BillRate,
					SUM(PFR.ForecastedHours) AS ForecastedHours,
					ROUND(ISNULL(SUM(TEP.BillableHours),0), 2) AS BillableHours ,
					ROUND(ISNULL(SUM(TEP.NonBillableHours),0), 2) AS NonBillableHours
			FROM PersonForeCastedHoursForBillRate	PFR
			LEFT JOIN TimeEntryPersonsForBillRate TEP ON TEP.PersonId = PFR.PersonId AND TEP.ChargeCodeDate = PFR.Date AND PFR.ProjectId = TEP.ProjectId
			WHERE PFR.BillRate IS NOT NULL
			GROUP BY PFR.ProjectId,PFR.BillRate)Proj
			GROUP BY Proj.ProjectId
		)
	 ,
	 ProjectForeCastedHoursUntilToday
	AS (
		SELECT	ProjectId,
				ClientId,
				ROUND(SUM(ForecastedHoursUntilToday),2) AS ForecastedHoursUntilToday,
				ROUND(SUM(ForecastedHours),2) AS ForecastedHours,
				MIN(MinimumValue) AS MinimumValue,
				MAX(MaximumValue) AS MaximumValue
		FROM PersonForeCastedHoursForBillRate	
		GROUP BY ProjectId,
				ClientId
	)
	,
	HoursData
	AS ( SELECT ProjectId,
			    ClientId,
				ROUND(SUM(BillableHours),2) AS BillableHours,
				ROUND(SUM(BillableHoursUntilToday),2) AS BillableHoursUntilToday,
				ROUND(SUM(NonBillableHours),2) AS NonBillableHours,
				GroupId,
			    TimeEntrySectionId
		 FROM  TimeEntryPersonsForBillRate
		 GROUP BY
			 ProjectId,
			 ClientId,
		     TimeEntrySectionId,
		     GroupId
	)

SELECT C.ClientId
		 , C.Name AS ClientName
		 , C.Code AS ClientCode
		 , PG.GroupId AS GroupId
		 , PG.Name AS GroupName
		 , PG.Code AS GroupCode
		 , P.ProjectId
		 , P.Name AS ProjectName
		 , P.ProjectNumber
		 , PS.ProjectStatusId
		 , PS.Name AS ProjectStatusName
		 , ISNULL(HD.BillableHours,0) AS BillableHours 
		 , ISNULL(HD.NonBillableHours,0) AS NonBillableHours
		 , ISNULL(pfh.ForecastedHoursUntilToday, 0) AS ForecastedHoursUntilToday
		 , ISNULL(pfh.ForecastedHours, 0) AS ForecastedHours
		 , ISNULL(HD.BillableHoursUntilToday,0) AS BillableHoursUntilToday
		 , ISNULL(HD.TimeEntrySectionId,-1) AS TimeEntrySectionId
		 , (CASE
			   WHEN pfh.MinimumValue IS NULL THEN
				   ''
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 0 THEN
			       'Fixed'
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 1 THEN
			       'Hourly'
			   WHEN  pfh.MinimumValue IS NOT NULL AND (pfh.MinimumValue <> PFH.MaximumValue) THEN
			       'Both'
		   END) AS BillingType
		 , (CASE
			  WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 0 THEN
			       -1
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 1 THEN
			       ISNULL(EBP.EstBillings,0)
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue <> PFH.MaximumValue THEN
			       ISNULL(EBP.EstBillings,0)
			   ELSE
				   ISNULL(EBP.EstBillings,0)
		   END) AS EstimatedBillings
		, CASE WHEN ISNULL(HD.TimeEntrySectionId,-1) = 2 THEN 1 ELSE ISNULL(P.IsBusinessDevelopment,0) END AS IsBusinessDevelopment
	FROM
		HoursData HD
		FULL JOIN ProjectForeCastedHoursUntilToday pfh
			ON pfh.ProjectId = HD.ProjectId AND pfh.ClientId = HD.ClientId
		LEFT JOIN dbo.Project P 
			ON P.ProjectId = ISNULL(pfh.ProjectId,HD.ProjectId)
		LEFT JOIN EstimatedBillingsByProject EBP 
			ON EBP.ProjectId = P.ProjectId
		LEFT JOIN Client C
			ON C.ClientId = ISNULL(pfh.ClientId,HD.ClientId)
		LEFT JOIN ProjectStatus PS
			ON PS.ProjectStatusId = P.ProjectStatusId
		LEFT JOIN ProjectGroup PG
			ON PG.GroupId = ISNULL(P.GroupId,HD.GroupId)
		
	WHERE
		((@ProjectBillingTypes IS NULL)
		OR ((CASE
			   WHEN (pfh.MinimumValue IS NULL) THEN
				   ''
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 0 THEN
			       'Fixed'
			   WHEN  pfh.MinimumValue IS NOT NULL AND pfh.MinimumValue = PFH.MaximumValue AND pfh.MinimumValue = 1 THEN
			       'Hourly'
			   ELSE
			       'Both'
			   
		   END) IN (SELECT PBT.BillingType
					 FROM @ProjectBillingTypesTable PBT )
			)
		)
		AND (@ProjectStatusIds IS NULL
					 OR P.ProjectStatusId IN (SELECT Ids
												FROM @ProjectStatusIdsTable )
				)
		AND P.ProjectNumber != 'P031000'	
	ORDER BY  C.Name,PG.Name

	;WITH ProjectForeCastedHoursUntilToday
	AS (
				SELECT M.ProjectId
					 , MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue
					 , MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
				FROM
					dbo.Milestone AS M
				GROUP BY
					M.ProjectId
	),

	PersonsCountCTE
	AS ( SELECT 
	            COUNT(DISTINCT P.PersonId) AS PersonsCount
		 FROM dbo.Project Pro
			  INNER JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
			  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
			  INNER JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
			  INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId
			  INNER JOIN dbo.Client C ON C.ClientId = Pro.ClientId
			  LEFT JOIN ProjectForeCastedHoursUntilToday pfh
				 ON pfh.ProjectId = PRO.ProjectId 
		 WHERE  MPE.StartDate <= @EndDateLocal AND @StartDateLocal <= MPE.EndDate
				 AND Pro.ClientId = @AccountIdLocal
				 AND (
						@BusinessUnitIds IS NULL
						 OR Pro.GroupId IN (SELECT Ids
												  FROM @BusinessUnitIdsTable )
					)
				 AND (@ProjectStatusIds IS NULL
						 OR Pro.ProjectStatusId IN (SELECT Ids
													FROM @ProjectStatusIdsTable )
					)
				 AND ((@ProjectBillingTypes IS NULL)
				 OR ((CASE
					 WHEN (pfh.MinimumValue IS NULL) THEN
						 ''
					 WHEN (pfh.MinimumValue = pfh.MaximumValue
						 AND pfh.MinimumValue = 0) THEN
						 'Fixed'
					 WHEN (pfh.MinimumValue = pfh.MaximumValue
						 AND pfh.MinimumValue = 1) THEN
						 'Hourly'
					 ELSE
						 'Both'
					 END) IN (SELECT PBT.BillingType
							  FROM @ProjectBillingTypesTable PBT )
					)
			)
	)

	SELECT C.Name AS ClientName
		 , C.Code AS ClientCode
		 , C.ClientId AS ClientId
		 , PC.PersonsCount
	FROM dbo.Client C
		INNER JOIN PersonsCountCTE AS PC ON 1 = 1
	WHERE C.ClientId = @AccountId
END	

