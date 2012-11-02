-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 04-03-2012
-- Updated by : Srinivas.M
-- Update Date: 05-21-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryByResourcePayCheck]
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

		DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME

		SET @StartDateLocal = CONVERT(DATE, @StartDate)
		SET @EndDateLocal = CONVERT(DATE, @EndDate)

		DECLARE @NOW DATE ,
			@HolidayTimeType INT ,
			@FutureDate DATETIME

		SELECT @NOW = dbo.GettingPMTime(GETUTCDATE()),
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

	DECLARE @HolidayTimeTypeId INT ,
			@PTOTimeTypeId INT ,
			@SickLeaveTimeTypeId INT ,
			@ORTTimeTypeId INT,
			@UnpaidTimeTypeId	INT
 		    
			 
	SELECT  @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId() ,
			@PTOTimeTypeId = dbo.GetPTOTimeTypeId() ,
			@ORTTimeTypeId = dbo.GetORTTimeTypeId() ,
			@SickLeaveTimeTypeId = dbo.[GetSickLeaveTimeTypeId](),
			@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()

	SELECT 	TT.TimeTypeId,
			TT.name,
			CASE WHEN TT.TimeTypeId = @PTOTimeTypeId THEN 1 ELSE 0  END IsPTO,
			CASE WHEN TT.TimeTypeId = @HolidayTimeTypeId THEN 1 ELSE 0  END IsHoliday,
			CASE WHEN TT.TimeTypeId = @ORTTimeTypeId THEN 1 ELSE 0  END IsORT,
			CASE WHEN TT.TimeTypeId = @SickLeaveTimeTypeId THEN 1 ELSE 0  END IsSickLeave,
			CASE WHEN TT.TimeTypeId = @UnpaidTimeTypeId THEN 1 ELSE 0  END IsUnpaid,
			CASE WHEN TT.Code = 'W9330' THEN 1 ELSE 0  END IsJuryDuty,
			CASE WHEN TT.Code = 'W9340' THEN 1 ELSE 0  END IsBereavement
	FROM dbo.TimeType TT
	WHERE TT.IsAdministrative = 1
		
		;WITH PersonPayDuringSelectedRange
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
					),
			ActivePersonsInSelectedRange
				AS ( SELECT DISTINCT
							PTSH.PersonId
					FROM     dbo.PersonStatusHistory PTSH
					WHERE    PTSH.StartDate < @EndDateLocal
							AND @StartDateLocal <  ISNULL(PTSH.EndDate,@FutureDate)
							AND PTSH.PersonStatusId IN (1,5) --ACTIVE STATUS
								
					)
			SELECT  1 AS BranchID ,
					CASE WHEN P.DefaultPractice = 4 THEN 100
						 ELSE 200
					END AS DeptID ,
					P.PersonId ,
					P.LastName ,
					P.FirstName ,
					P.EmployeeNumber ,
					P.PaychexID ,
					ISNULL(Data.TotalHours, 0) AS TotalHours ,
					ISNULL(Data.PTOHours, 0) AS PTOHours ,
					ISNULL(Data.HolidayHours, 0) AS HolidayHours ,
					ISNULL(Data.JuryDutyHours, 0) AS JuryDutyHours ,
					ISNULL(Data.BereavementHours, 0) AS BereavementHours ,
					ISNULL(Data.ORTHours, 0) AS ORTHours ,
					ISNULL(Data.UnpaidHours, 0) AS UnpaidHours ,
					ISNULL(Data.SickOrSafeLeaveHours, 0) AS SickOrSafeLeaveHours ,
					PCP.Timescale,
					P.DivisionId
			FROM    ( SELECT    TE.PersonId ,
								ROUND(SUM(CASE WHEN CC.TimeEntrySectionId <> 4
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS TotalHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9310'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS PTOHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9320'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS HolidayHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9330'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS JuryDutyHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9340'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS BereavementHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9300'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS ORTHours ,
								ROUND(SUM(CASE WHEN TT.Code = 'W9350'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS UnpaidHours,
								ROUND(SUM(CASE WHEN TT.Code = 'W9311'
											   THEN TEH.ActualHours
											   ELSE 0
										  END), 2) AS SickOrSafeLeaveHours
					  FROM      dbo.TimeEntry TE
								INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
															  AND TE.ChargeCodeDate BETWEEN @StartDateLocal
															  AND
															  @EndDateLocal
								INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
								INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
								INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId
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
											   AND P.IsStrawman = 0
					INNER JOIN PersonWithPay PCP ON P.PersonId = PCP.PersonId
					INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
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
								 OR P.PersonStatusId IN (
								 SELECT  ResultId
								 FROM    dbo.ConvertStringListIntoTable(@PersonStatusIds) )
							  )
						  AND ( @TimeScaleNamesList IS NULL
								OR ( CASE WHEN P.PersonStatusId = 2
										  THEN 'Terminated'
										  ELSE ISNULL(PCP.Timescale, '')
									 END ) IN ( SELECT  Name
												FROM    @TimeScaleNames )
							  )
						  AND ( @PersonDivisionIds IS NULL
								OR ISNULL(P.DivisionId, '') IN (SELECT Id
																FROM @DivisionIds)
							  )
						)
			ORDER BY P.LastName ,
					P.FirstName
END

