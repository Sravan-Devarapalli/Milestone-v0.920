-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : ThulasiRam.P
-- Update Date: 06-05-2012
-- Description:  Time Entries grouped by Project for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByProject]
	(
	  @StartDate DATETIME ,
	  @EndDate DATETIME ,
	  @ClientIds NVARCHAR(MAX) = NULL ,
	  @ProjectStatusIds NVARCHAR(MAX) = NULL
	)
AS 
	BEGIN

		DECLARE @Today DATE ,
			@StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@HolidayTimeType INT ,
			@FutureDate DATETIME

		SELECT @StartDateLocal = CONVERT(DATE, @StartDate), @EndDateLocal = CONVERT(DATE, @EndDate),@Today = dbo.GettingPMTime(GETUTCDATE()), @HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate()

		DECLARE @ClientIdsTable TABLE ( ID INT )
		INSERT  INTO @ClientIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ClientIds)

		DECLARE @ProjectStatusIdsTable TABLE ( ID INT )
		INSERT  INTO @ProjectStatusIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ProjectStatusIds);
			WITH    ProjectForeCastedHoursUntilToday
					  AS ( SELECT   M.ProjectId ,
					                SUM(CASE WHEN PC.Date < @Today THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.ActualHours,MPE.HoursPerDay))
											 ELSE 0
										END)  AS ForecastedHoursUntilToday,
									SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.ActualHours,MPE.HoursPerDay)) AS ForecastedHours,
									MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
									MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
						   FROM     dbo.MilestonePersonEntry AS MPE
									INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
									INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
									INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
									INNER JOIN dbo.v_PersonCalendar PC ON PC.PersonId = MP.PersonId
															  --AND PC.DayOff = 0
															  AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate
															  AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal															  
						   GROUP BY M.ProjectId
						 )
			SELECT  C.ClientId ,
					C.Name AS ClientName ,
					C.Code AS ClientCode ,
					PG.GroupId, 
					PG.Name AS GroupName ,
					PG.Code AS GroupCode ,
					P.ProjectId ,
					P.Name AS ProjectName ,
					P.ProjectNumber ,
					PS.ProjectStatusId ,
					PS.Name AS ProjectStatusName ,
					BillableHours ,
					NonBillableHours ,
					ISNULL(pfh.ForecastedHoursUntilToday, 0) AS ForecastedHoursUntilToday ,
					ISNULL(pfh.ForecastedHours, 0) AS ForecastedHours ,
					BillableHoursUntilToday ,
					TimeEntrySectionId ,
					( CASE WHEN ( pfh.MinimumValue IS NULL ) THEN ''
						   WHEN ( pfh.MinimumValue = pfh.MaximumValue
								  AND pfh.MinimumValue = 0
								) THEN 'Fixed'
						   WHEN ( pfh.MinimumValue = pfh.MaximumValue
								  AND pfh.MinimumValue = 1
								) THEN 'Hourly'
						   ELSE 'Both'
					  END ) AS BillingType
			FROM    ( SELECT    CC.ClientId ,
								CC.ProjectId ,
								CC.ProjectGroupId ,
								CC.TimeEntrySectionId ,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
													AND PRO.ProjectNumber != 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS [BillableHours] ,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
													OR PRO.ProjectNumber = 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS [NonBillableHours] ,
								ROUND(SUM(CASE WHEN ( TEH.IsChargeable = 1
													  AND PRO.ProjectNumber != 'P031000'
													  AND TE.ChargeCodeDate < @Today
													) THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS BillableHoursUntilToday
					  FROM      dbo.TimeEntry TE
								INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
															  AND TE.ChargeCodeDate BETWEEN @StartDateLocal
															  AND
															  @EndDateLocal
								INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
								INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
								INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId
								INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = P.PersonId
															  AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate)
					  WHERE     TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
															@FutureDate)
								AND ( CC.timeTypeId != @HolidayTimeType
									  OR ( CC.timeTypeId = @HolidayTimeType
										   AND PTSH.PersonStatusId IN (1,5)
										 )
									)
					  GROUP BY  CC.TimeEntrySectionId ,
								CC.ClientId ,
								CC.ProjectGroupId ,
								CC.ProjectId
					) Data
					INNER JOIN dbo.Project P ON P.ProjectId = Data.ProjectId
					INNER JOIN dbo.Client C ON C.ClientId = Data.ClientId
					INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId
					INNER JOIN dbo.ProjectGroup PG ON PG.ClientId = C.ClientId
													  AND PG.GroupId = Data.ProjectGroupId
					LEFT JOIN ProjectForeCastedHoursUntilToday pfh ON pfh.ProjectId = P.ProjectId
			WHERE   ( @ClientIds IS NULL
					  OR C.ClientId IN ( SELECT ID
										 FROM   @ClientIdsTable )
					)
					AND ( @ProjectStatusIds IS NULL
						  OR PS.ProjectStatusId IN (
						  SELECT    ID
						  FROM      @ProjectStatusIdsTable )
						)
			ORDER BY TimeEntrySectionId ,
					P.ProjectNumber
	
	END
	

