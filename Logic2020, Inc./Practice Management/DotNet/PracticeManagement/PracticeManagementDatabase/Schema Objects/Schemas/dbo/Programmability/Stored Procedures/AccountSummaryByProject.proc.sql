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
		@HolidayTimeType INT

SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
	 , @EndDateLocal = CONVERT(DATE, @EndDate)
	 , @Today = dbo.GettingPMTime(GETUTCDATE())
	 , @HolidayTimeType = dbo.GetHolidayTimeTypeId()

	;WITH PersonTotalStatusHistory
    AS (
SELECT PSH.PersonId
	 , CASE
		   WHEN PSH.StartDate = MinPayStartDate THEN
			   PS.HireDate
		   ELSE
			   PSH.StartDate
	   END AS StartDate
	 , ISNULL(PSH.EndDate, dbo.GetFutureDate()) AS EndDate
	 , PSH.PersonStatusId
FROM
	dbo.PersonStatusHistory PSH
	LEFT JOIN (SELECT PSH.PersonId
					, P.HireDate
					, MIN(PSH.StartDate) AS MinPayStartDate
			   FROM
				   dbo.PersonStatusHistory PSH
				   INNER JOIN dbo.Person P
					   ON PSH.PersonId = P.PersonId
			   WHERE
				   P.IsStrawman = 0
			   GROUP BY
				   PSH.PersonId
				 , P.HireDate
			   HAVING
				   P.HireDate < MIN(PSH.StartDate)) AS PS
		ON PS.PersonId = PSH.PersonId
WHERE
	PSH.StartDate < @EndDateLocal
	AND @StartDateLocal < ISNULL(PSH.EndDate, dbo.GetFutureDate())
        ),
	ProjectForeCastedHoursUntilToday
    AS ( SELECT M.ProjectId
			  , SUM(MPE.HoursPerDay) AS ForecastedHoursUntilToday
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
			 INNER JOIN dbo.v_PersonCalendar PC
				 ON PC.PersonId = MP.PersonId AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate AND PC.Date BETWEEN @StartDateLocal AND CASE
					 WHEN @EndDateLocal > DATEADD(DAY, -1, @Today) THEN
						 DATEADD(DAY, -1, @Today)
					 ELSE
						 @EndDateLocal
				 END AND ((PC.CompanyDayOff = 0 AND ISNULL(PC.TimeTypeId, 0) != dbo.GetHolidayTimeTypeId()) OR (PC.CompanyDayOff = 1 AND PC.SubstituteDate IS NOT NULL))
		 GROUP BY
			 M.ProjectId
),
HoursData
AS (SELECT PRO.ProjectId
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
		INNER JOIN PersonTotalStatusHistory PTSH
			ON PTSH.PersonId = P.PersonId AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND PTSH.EndDate
		INNER JOIN dbo.Project PRO
			ON PRO.ProjectId = CC.ProjectId
	WHERE
		CC.ClientId = @AccountId
		AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, dbo.GetFutureDate())
		AND (CC.timeTypeId != @HolidayTimeType
		OR (CC.timeTypeId = @HolidayTimeType
		AND PTSH.PersonStatusId = 1))
		AND (@BusinessUnitIds IS NULL
		OR CC.ProjectGroupId IN (SELECT ResultId
								 FROM
									 dbo.ConvertStringListIntoTable(@BusinessUnitIds)))
		AND (@ProjectStatusIds IS NULL
		OR PRO.ProjectStatusId IN (SELECT ResultId
								   FROM
									   dbo.ConvertStringListIntoTable(@ProjectStatusIds)))
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
	END) IN (SELECT ResultString
			 FROM
				 [dbo].[ConvertXmlStringInToStringTable](@ProjectBillingTypes))))
ORDER BY
	HD.TimeEntrySectionId
  , HD.ProjectNumber
	
END
