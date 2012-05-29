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
		@HolidayTimeType INT

	SET @StartDateLocal = CONVERT(DATE, @StartDate)
	SET @EndDateLocal = CONVERT(DATE, @EndDate)
	SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()
	
	;WITH PersonTotalStatusHistory
    AS ( SELECT   PSH.PersonId ,
                CASE WHEN PSH.StartDate = MinPayStartDate
                        THEN PS.HireDate
                        ELSE PSH.StartDate
                END AS StartDate ,
                ISNULL(PSH.EndDate, dbo.GetFutureDate()) AS EndDate ,
                PSH.PersonStatusId
        FROM     dbo.PersonStatusHistory PSH
                LEFT JOIN ( SELECT  PSH.PersonId ,
                                    P.HireDate ,
                                    MIN(PSH.StartDate) AS MinPayStartDate
                            FROM    dbo.PersonStatusHistory PSH
                                    INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId
                            WHERE   P.IsStrawman = 0
                            GROUP BY PSH.PersonId ,
                                    P.HireDate
                            HAVING  P.HireDate < MIN(PSH.StartDate)
                            ) AS PS ON PS.PersonId = PSH.PersonId
        WHERE    CASE WHEN PSH.StartDate = MinPayStartDate
                        THEN PS.HireDate
                        ELSE PSH.StartDate
                END < @EndDateLocal
                AND @StartDateLocal < ISNULL(PSH.EndDate,
                                                dbo.GetFutureDate())
        )

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
                INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = P.PersonId
                                                AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																			AND PTSH.EndDate
				INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
				LEFT JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
        WHERE CC.ClientId = @AccountId
				AND	TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
                                            dbo.GetFutureDate())
                AND ( CC.timeTypeId != @HolidayTimeType
                        OR ( CC.timeTypeId = @HolidayTimeType
                            AND PTSH.PersonStatusId = 1
                            )
                    )
				AND (@BusinessUnitIds IS NULL
					OR CC.ProjectGroupId IN (SELECT *
											FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds)
											)
					)
        GROUP BY  PG.GroupId, PG.Name, PG.Active, PG.Code

		
	;WITH PersonTotalStatusHistory
    AS ( SELECT   PSH.PersonId ,
                CASE WHEN PSH.StartDate = MinPayStartDate
                        THEN PS.HireDate
                        ELSE PSH.StartDate
                END AS StartDate ,
                ISNULL(PSH.EndDate, dbo.GetFutureDate()) AS EndDate ,
                PSH.PersonStatusId
        FROM     dbo.PersonStatusHistory PSH
                LEFT JOIN ( SELECT  PSH.PersonId ,
                                    P.HireDate ,
                                    MIN(PSH.StartDate) AS MinPayStartDate
                            FROM    dbo.PersonStatusHistory PSH
                                    INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId
                            WHERE   P.IsStrawman = 0
                            GROUP BY PSH.PersonId ,
                                    P.HireDate
                            HAVING  P.HireDate < MIN(PSH.StartDate)
                            ) AS PS ON PS.PersonId = PSH.PersonId
        WHERE    CASE WHEN PSH.StartDate = MinPayStartDate
                        THEN PS.HireDate
                        ELSE PSH.StartDate
                END < @EndDateLocal
                AND @StartDateLocal < ISNULL(PSH.EndDate,
                                                dbo.GetFutureDate())
        )

		SELECT COUNT(DISTINCT TE.PersonId) AS PersonsCount,
			C.Name AS ClientName,
			C.Code AS ClientCode,
			C.ClientId AS ClientId
        FROM    dbo.TimeEntry TE
                INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
                                                AND TE.ChargeCodeDate BETWEEN @StartDateLocal
                                                AND
                                                @EndDateLocal
                INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
                INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
                INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = P.PersonId
                                                AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
																			AND PTSH.EndDate
				INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
				LEFT JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId
				INNER JOIN dbo.Client C ON C.ClientId = CC.ClientId
        WHERE CC.ClientId = @AccountId
				AND	TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
                                            dbo.GetFutureDate())
                AND ( CC.timeTypeId != @HolidayTimeType
                        OR ( CC.timeTypeId = @HolidayTimeType
                            AND PTSH.PersonStatusId = 1
                            )
                    )
				AND (@BusinessUnitIds IS NULL
					OR CC.ProjectGroupId IN (SELECT *
											FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds)
											)
					)
		GROUP BY C.ClientId, C.Name, C.Code

END
