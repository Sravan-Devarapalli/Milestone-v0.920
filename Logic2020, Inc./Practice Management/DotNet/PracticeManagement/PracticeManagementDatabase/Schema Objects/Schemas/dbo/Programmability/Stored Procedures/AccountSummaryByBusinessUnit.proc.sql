CREATE PROCEDURE [dbo].[AccountSummaryByBusinessUnit]
(
	@AccountId	INT,
	@BusinessUnitIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	
	
	DECLARE @StartDateLocal DATETIME ,
		@EndDateLocal DATETIME,
		@HolidayTimeType INT,
		@FutureDate DATETIME

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate), @EndDateLocal = CONVERT(DATE, @EndDate), @HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate()

	DECLARE @BusinessUnitIdsTable TABLE ( Id INT)

	INSERT INTO @BusinessUnitIdsTable(Id)
	SELECT BU.ResultId
	FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds) BU
	
	
		SELECT    PG.GroupId, 
					PG.Name AS GroupName, 
					PG.Active,
					PG.Code AS GroupCode,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
										AND Pro.ProjectNumber != 'P031000'
									THEN TEH.ActualHours
									ELSE 0
								END), 2) AS BillableHours ,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
										AND CC.TimeEntrySectionId <> 2
										AND Pro.ProjectNumber != 'P031000'
									THEN TEH.ActualHours
									ELSE 0
								END), 2) AS NonBillableHours ,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 2
									THEN TEH.ActualHours
									ELSE 0
								END), 2) AS BusinessDevelopmentHours,
					COUNT(DISTINCT CC.ProjectId) AS ProjectsCount
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
				LEFT JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
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
		GROUP BY  PG.GroupId, PG.Name, PG.Active, PG.Code

		
	

		SELECT COUNT(DISTINCT TE.PersonId) AS PersonsCount,
			C.Name AS ClientName,
			C.Code AS ClientCode,
			C.ClientId AS ClientId
		FROM    dbo.TimeEntry TE
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
												AND TE.ChargeCodeDate BETWEEN @StartDateLocal
												AND
												@EndDateLocal
				INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
				INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = P.PersonId
												AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																			AND ISNULL(PTSH.EndDate,@FutureDate)
				INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
				LEFT JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
				INNER JOIN dbo.Client C ON C.ClientId = CC.ClientId
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
		GROUP BY C.ClientId, C.Name, C.Code

END
