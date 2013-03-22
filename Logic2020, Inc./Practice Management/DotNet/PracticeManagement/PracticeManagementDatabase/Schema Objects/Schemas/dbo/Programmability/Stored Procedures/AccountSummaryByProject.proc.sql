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
		@FutureDate DATETIME

	DECLARE @ProjectBillingTypesTable TABLE( BillingType NVARCHAR(100) )
	DECLARE @BusinessUnitIdsTable TABLE ( Ids INT )
	DECLARE @ProjectStatusIdsTable TABLE ( Ids INT )

	INSERT INTO @ProjectBillingTypesTable(BillingType)
	SELECT ResultString
	FROM [dbo].[ConvertXmlStringInToStringTable](@ProjectBillingTypes)

	INSERT INTO @BusinessUnitIdsTable(Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds)

	INSERT INTO @ProjectStatusIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectStatusIds)


	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)
		 , @Today = dbo.GettingPMTime(GETUTCDATE())
		 , @HolidayTimeType = dbo.GetHolidayTimeTypeId()
		 , @FutureDate = dbo.GetFutureDate()

	;WITH ProjectForeCastedHoursUntilToday
	AS (
		SELECT M.ProjectId
				, SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ForecastedHoursUntilToday
				, MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue
				, MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
		FROM
			dbo.MilestonePersonEntry AS MPE
			INNER JOIN dbo.MilestonePerson AS MP
				ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone AS M
				ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.person AS P
				ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
			INNER JOIN dbo.PersonCalendarAuto PC
				ON PC.PersonId = MP.PersonId AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate AND PC.Date BETWEEN @StartDateLocal AND CASE
					WHEN @EndDateLocal > DATEADD(DAY, -1, @Today) THEN
						DATEADD(DAY, -1, @Today)
					ELSE
						@EndDateLocal
				END
		GROUP BY
			M.ProjectId
	),
	HoursData
	AS ( SELECT PRO.ProjectId
			  , CC.ClientId
			  , PRO.ProjectStatusId
			  , PRO.Name AS ProjectName
			  , PRO.ProjectNumber
			  , CC.ProjectGroupId AS GroupId
			  , CC.TimeEntrySectionId
			  , ROUND(SUM(CASE
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
					WHEN (TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' AND TE.ChargeCodeDate < @Today) THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS BillableHoursUntilToday
		 FROM
			 dbo.TimeEntry TE
			 INNER JOIN dbo.TimeEntryHours TEH
				 ON TEH.TimeEntryId = TE.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
			 INNER JOIN dbo.ChargeCode CC
				 ON CC.Id = TE.ChargeCodeId
			 INNER JOIN dbo.Person P
				 ON P.PersonId = TE.PersonId
			 INNER JOIN dbo.PersonStatusHistory PTSH
				 ON PTSH.PersonId = P.PersonId AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND ISNULL(PTSH.EndDate,@FutureDate)
			 INNER JOIN dbo.Project PRO
				 ON PRO.ProjectId = CC.ProjectId
		 WHERE
			 CC.ClientId = @AccountId
			 AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, @FutureDate)
			 AND (CC.timeTypeId != @HolidayTimeType
			 OR (CC.timeTypeId = @HolidayTimeType
			 AND PTSH.PersonStatusId IN (1,5) -- ACTIVE And Terminated Pending
			 ))
			 AND (@BusinessUnitIds IS NULL
					OR CC.ProjectGroupId IN (SELECT Ids
											FROM @BusinessUnitIdsTable )
				)
			 AND (@ProjectStatusIds IS NULL
					 OR PRO.ProjectStatusId IN (SELECT Ids
												FROM @ProjectStatusIdsTable )
				)
		 GROUP BY
			 PRO.ProjectId
		   , CC.ClientId
		   , CC.TimeEntrySectionId
		   , PRO.ProjectStatusId
		   , PRO.Name
		   , PRO.ProjectNumber
		   , CC.ProjectGroupId

	)
	SELECT C.ClientId
		 , C.Name AS ClientName
		 , C.Code AS ClientCode
		 , PG.GroupId AS GroupId
		 , PG.Name AS GroupName
		 , PG.Code AS GroupCode
		 , HD.ProjectId
		 , HD.ProjectName
		 , HD.ProjectNumber
		 , PS.ProjectStatusId
		 , PS.Name AS ProjectStatusName
		 , HD.BillableHours
		 , HD.NonBillableHours
		 , ISNULL(pfh.ForecastedHoursUntilToday, 0) AS ForecastedHoursUntilToday
		 , BillableHoursUntilToday
		 , HD.TimeEntrySectionId
		 , (CASE
			   WHEN (pfh.MinimumValue IS NULL) THEN
				   ''
			   WHEN (pfh.MinimumValue = pfh.MaximumValue AND pfh.MinimumValue = 0) THEN
				   'Fixed'
			   WHEN (pfh.MinimumValue = pfh.MaximumValue AND pfh.MinimumValue = 1) THEN
				   'Hourly'
			   ELSE
				   'Both'
		   END) AS BillingType
	FROM
		HoursData HD
		INNER JOIN Client C
			ON C.ClientId = HD.ClientId
		INNER JOIN ProjectStatus PS
			ON PS.ProjectStatusId = HD.ProjectStatusId
		INNER JOIN ProjectGroup PG
			ON PG.GroupId = HD.GroupId
		LEFT JOIN ProjectForeCastedHoursUntilToday pfh
			ON pfh.ProjectId = HD.ProjectId
	WHERE
		((@ProjectBillingTypes IS NULL)
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
	ORDER BY
		HD.TimeEntrySectionId
	  , HD.ProjectNumber

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
	AS ( SELECT COUNT(DISTINCT TE.PersonId) AS PersonsCount
		 FROM
			 dbo.TimeEntry TE
			 INNER JOIN dbo.ChargeCode CC
				 ON CC.Id = TE.ChargeCodeId
			 INNER JOIN dbo.Person P
				 ON P.PersonId = TE.PersonId
			 INNER JOIN dbo.PersonStatusHistory PTSH
				 ON PTSH.PersonId = P.PersonId AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND ISNULL(PTSH.EndDate,@FutureDate)
			 INNER JOIN dbo.Project PRO
				 ON PRO.ProjectId = CC.ProjectId
			 LEFT JOIN ProjectForeCastedHoursUntilToday pfh
				 ON pfh.ProjectId = PRO.ProjectId
		 WHERE  TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal AND
			 CC.ClientId = @AccountId
			 AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, @FutureDate)
			 AND (CC.timeTypeId != @HolidayTimeType
			 OR (CC.timeTypeId = @HolidayTimeType
			 AND PTSH.PersonStatusId IN (1,5) -- ACTIVE And Terminated Pending
			 ))
			 AND (@BusinessUnitIds IS NULL
					 OR CC.ProjectGroupId IN (SELECT Ids
											  FROM @BusinessUnitIdsTable )
				)
			 AND (@ProjectStatusIds IS NULL
					 OR PRO.ProjectStatusId IN (SELECT Ids
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

