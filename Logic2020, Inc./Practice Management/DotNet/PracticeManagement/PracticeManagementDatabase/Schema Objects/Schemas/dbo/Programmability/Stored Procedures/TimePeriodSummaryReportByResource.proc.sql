-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Srinivas.M
-- Update Date: 06-19-2012
-- Description:  Time Entries grouped by Resource for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByResource]
	(
	  @StartDate DATETIME ,
	  @EndDate DATETIME ,
	  @IncludePersonsWithNoTimeEntries BIT ,
	  @PersonTypes NVARCHAR(MAX) = NULL ,
	  @SeniorityIds NVARCHAR(MAX) = NULL ,
	  @TimeScaleNamesList XML = NULL,
	  @PersonStatusIds NVARCHAR(MAX) = NULL,
	  @PersonDivisionIds NVARCHAR(MAX) = NULL
	)
AS 
	BEGIN
		SET NOCOUNT ON;
		DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@NOW DATE ,
			@HolidayTimeType INT ,
			@FutureDate DATETIME

		SELECT @StartDateLocal = CONVERT(DATE, @StartDate), 
			   @EndDateLocal = CONVERT(DATE, @EndDate),
			   @NOW = dbo.GettingPMTime(GETUTCDATE()),
			   @HolidayTimeType = dbo.GetHolidayTimeTypeId(),
			   @FutureDate = dbo.GetFutureDate()


		DECLARE @TimeScaleNames TABLE ( Name NVARCHAR(1024) )
	
		INSERT  INTO @TimeScaleNames
				SELECT  ResultString
				FROM    [dbo].[ConvertXmlStringInToStringTable](@TimeScaleNamesList)

		DECLARE @DivisionIds TABLE ( Id NVARCHAR(20))

		INSERT INTO @DivisionIds
				SELECT ResultString
				FROM	[dbo].[ConvertXmlStringInToStringTable](@PersonDivisionIds)

	-- Get person level Default hours in between the StartDate and EndDate
	--1.Day should not be company holiday and also not converted to substitute day.
	--2.day should be company holiday and it should be taken as a substitute holiday.
	;
		WITH    PersonDefaultHoursWithInPeriod
				  AS ( SELECT   Pc.Personid ,
								( COUNT(PC.Date) * 8 ) AS DefaultHours --Estimated working hours per day is 8.
					   FROM     ( SELECT    CAL.Date ,
											P.PersonId ,
											CAL.DayOff AS CompanyDayOff ,
											PCAL.TimeTypeId ,
											PCAL.SubstituteDate
								  FROM      dbo.Calendar AS CAL
											INNER JOIN dbo.Person AS P ON CAL.Date >= P.HireDate
															  AND CAL.Date <= ISNULL(P.TerminationDate,
															  @FutureDate)
											LEFT JOIN dbo.PersonCalendar AS PCAL ON PCAL.Date = CAL.Date
															  AND PCAL.PersonId = P.PersonId
								) AS PC
					   WHERE    PC.Date BETWEEN @StartDateLocal
										AND     CASE WHEN @EndDateLocal > DATEADD(day,
															  -1, @NOW)
													 THEN DATEADD(day, -1,
															  @NOW)
													 ELSE @EndDateLocal
												END
								AND ( ( PC.CompanyDayOff = 0
										AND ISNULL(PC.TimeTypeId, 0) != @HolidayTimeType
									  )
									  OR ( PC.CompanyDayOff = 1
										   AND PC.SubstituteDate IS NOT NULL
										 )
									)
					   GROUP BY PC.PersonId
					 ),
				ActivePersonsInSelectedRange
				  AS ( SELECT DISTINCT
								PTSH.PersonId
					   FROM     dbo.PersonStatusHistory PTSH
					   WHERE    PTSH.StartDate < @EndDateLocal
								AND @StartDateLocal <  ISNULL(PTSH.EndDate,@FutureDate)
								AND PTSH.PersonStatusId IN (1,5) --ACTIVE STATUS
								
					 ),
				    PersonPayDuringSelectedRange
				  AS ( 
					   SELECT   P.PersonId ,
								MAX(pa.StartDate) AS StartDate
					   FROM     dbo.Person AS P
								LEFT JOIN dbo.Pay pa ON pa.Person = P.PersonId 
														AND pa.StartDate <= @EndDateLocal AND (ISNULL(pa.EndDate, @FutureDate) - 1) >= @StartDateLocal
					   WHERE    P.IsStrawman = 0
					   GROUP BY P.PersonId

					 ),
					 PersonPayToday
					 AS 
					 (
					   SELECT   P.PersonId ,
								MAX(pa.StartDate) AS StartDate
					   FROM     dbo.Person AS P
								LEFT JOIN dbo.Pay AS pa ON pa.Person = P.PersonId 
														AND @NOW BETWEEN pa.StartDate AND (ISNULL(pa.EndDate, @FutureDate) - 1)
					   WHERE    P.IsStrawman = 0
					   GROUP BY P.PersonId
					 )
					 ,
					 PersonWithPay AS
					 (
					  SELECT PPDTP.PersonId,
					         ISNULL(TS.Name,'') AS Timescale
					  FROM	PersonPayDuringSelectedRange AS PPDTP
					  LEFT JOIN dbo.Pay AS pa ON pa.Person = PPDTP.PersonId  AND pa.StartDate = PPDTP.StartDate
					  LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
					  WHERE PPDTP.StartDate IS NOT NULL
					  UNION 
					  SELECT PPT.PersonId,
					         ISNULL(TS.Name,'') AS Timescale
					  FROM	 PersonPayDuringSelectedRange AS PPDTP
					  INNER JOIN PersonPayToday AS PPT ON PPDTP.PersonId = PPT.PersonId
					  LEFT JOIN dbo.Pay AS pa ON pa.Person = PPT.PersonId  AND pa.StartDate = PPT.StartDate
					  LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
					  WHERE PPDTP.StartDate IS NULL
					 )

					 
			SELECT  P.PersonId ,
					P.LastName ,
					P.FirstName ,
					S.SeniorityId ,
					S.Name SeniorityName ,
					P.IsOffshore ,
					ISNULL(Data.BillableHours, 0) AS BillableHours ,
					ISNULL(Data.ProjectNonBillableHours, 0) AS ProjectNonBillableHours ,
					ISNULL(Data.BusinessDevelopmentHours, 0) AS BusinessDevelopmentHours ,
					ISNULL(Data.InternalHours, 0) AS InternalHours ,
					ISNULL(Data.AdminstrativeHours, 0) AS AdminstrativeHours ,
					ISNULL(ROUND(CASE WHEN ISNULL(PDH.DefaultHours, 0) = 0
									  THEN 0
									  ELSE ( Data.ActualHours * 100 )
										   / PDH.DefaultHours
								 END, 0), 0) AS UtlizationPercent ,
					PCP.Timescale,
					PS.PersonStatusId AS 'PersonStatusId',
					PS.Name AS 'PersonStatusName',
					P.DivisionId AS 'DivisionId'
			FROM    ( SELECT    TE.PersonId ,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
													AND Pro.ProjectNumber != 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS BillableHours ,
								ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
													AND CC.TimeEntrySectionId = 1
													AND Pro.ProjectNumber != 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS ProjectNonBillableHours ,
								ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 2
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS BusinessDevelopmentHours ,
								ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 3
													OR Pro.ProjectNumber = 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS InternalHours ,
								ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 4
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS AdminstrativeHours ,
								ROUND(SUM(CASE WHEN CC.TimeEntrySectionId = 1
													AND @NOW > TE.ChargeCodeDate
													AND Pro.ProjectNumber != 'P031000'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS ActualHours
					  FROM      dbo.TimeEntry TE
								INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
															  AND TE.ChargeCodeDate BETWEEN @StartDateLocal
															  AND
															  @EndDateLocal
								INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
								INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
								INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
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
					  GROUP BY  TE.PersonId
					) Data
					FULL JOIN ActivePersonsInSelectedRange AP ON AP.PersonId = Data.PersonId
					INNER JOIN dbo.Person P ON ( P.PersonId = Data.PersonId
												 OR AP.PersonId = P.PersonId
											   )
											   AND p.IsStrawman = 0
					INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
					INNER JOIN PersonWithPay PCP ON P.PersonId = PCP.PersonId 
					INNER JOIN [dbo].[PersonStatus] PS ON PS.PersonStatusId = P.PersonStatusId
					LEFT JOIN PersonDefaultHoursWithInPeriod PDH ON PDH.PersonId = P.PersonId
			WHERE   ( @IncludePersonsWithNoTimeEntries = 1
					  OR ( @IncludePersonsWithNoTimeEntries = 0
						   AND Data.PersonId IS NOT NULL
						 )
					)
					AND ( ( @PersonTypes IS NULL
							OR ( CASE WHEN P.IsOffshore = 1 THEN 'Offshore'
									  ELSE 'Domestic'
								 END ) IN (
							SELECT  ResultString
							FROM    [dbo].[ConvertXmlStringInToStringTable](@PersonTypes) )
						  )
						  AND ( @SeniorityIds IS NULL
								OR S.SeniorityId IN (
								SELECT  ResultId
								FROM    dbo.ConvertStringListIntoTable(@SeniorityIds) )
							  )
						   AND ( @PersonStatusIds IS NULL
								 OR PS.PersonStatusId IN (
								 SELECT  ResultId
								 FROM    dbo.ConvertStringListIntoTable(@PersonStatusIds) )
							  )
						  AND ( @TimeScaleNamesList IS NULL
								OR ( PCP.Timescale ) IN ( 
								  SELECT  Name FROM    @TimeScaleNames )
							  )
						  AND ( @PersonDivisionIds IS NULL
								OR ISNULL(P.DivisionId, '') IN (SELECT Id
																FROM @DivisionIds)
							  )
						)
			ORDER BY P.LastName ,
					P.FirstName
	END

