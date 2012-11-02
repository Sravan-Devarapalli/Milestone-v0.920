-- =============================================
-- Description:	Get PersonTimeEntriesByPeriod.
-- Updated by:	sainath c
-- Update date:	01-06-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonTimeEntriesByPeriod]
    @PersonId INT ,
    @StartDate DATETIME ,
    @EndDate DATETIME
AS 
    BEGIN
	
        DECLARE @HolidayTimeTypeId INT ,
            @PTOTimeTypeId INT ,
			@SickLeaveTimeTypeId INT ,
            @IsW2SalaryPerson BIT = 0 ,
			@IsW2HourlyPerson BIT = 0,
            @W2SalaryId INT ,
			@W2HourlyId INT ,
            @FutureDateLocal DATETIME ,
            @StartDateLocal DATETIME ,
            @EndDateLocal DATETIME ,
            @ORTTimeTypeId INT,
			@UnpaidTimeTypeId	INT
 		    
			 
        SELECT  @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId() ,
                @PTOTimeTypeId = dbo.GetPTOTimeTypeId() ,
                @ORTTimeTypeId = dbo.GetORTTimeTypeId() ,
                @FutureDateLocal = dbo.GetFutureDate() ,
				@SickLeaveTimeTypeId = dbo.[GetSickLeaveTimeTypeId](),
                @StartDateLocal = @StartDate ,
                @EndDateLocal = @EndDate,
				@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()
	
        SELECT  @W2SalaryId = TimescaleId FROM dbo.Timescale WHERE   Name = 'W2-Salary'
		SELECT  @W2HourlyId = TimescaleId FROM dbo.Timescale WHERE   Name = 'W2-Hourly'

        SELECT  @IsW2SalaryPerson = 1
        FROM    dbo.Pay pay
        WHERE   pay.Person = @PersonId
                AND pay.Timescale = @W2SalaryId
                AND pay.StartDate <= @EndDateLocal
                AND pay.EndDate-1 >= @StartDateLocal

        SELECT  @IsW2HourlyPerson = 1
        FROM    dbo.Pay pay
        WHERE   pay.Person = @PersonId
                AND pay.Timescale = @W2HourlyId
                AND pay.StartDate <= @EndDateLocal
                AND pay.EndDate-1 >= @StartDateLocal



	--List Of time entries with detail
        SELECT  TE.TimeEntryId ,
                TE.ChargeCodeId ,
                TE.ChargeCodeDate ,
                TEH.CreateDate ,
                TEH.ModifiedDate ,
                TEH.ActualHours ,
                TE.ForecastedHours ,
                ( CASE WHEN (CC.TimeTypeId = @HolidayTimeTypeId OR CC.TimeTypeId = @UnpaidTimeTypeId)
                       THEN TE.Note + dbo.GetApprovedByName(TE.ChargeCodeDate,
                                                            CC.TimeTypeId,
                                                            TE.PersonId)
                       ELSE TE.Note
                  END ) AS Note ,
                TEH.IsChargeable ,
                TEH.ReviewStatusId ,
                CC.TimeTypeId ,
                TEH.ModifiedBy ,
                PC.ApprovedBy 'ApprovedBy' ,
                AP.LastName 'ApprovedByLastName' ,
                AP.FirstName 'ApprovedByFirstName'
        FROM    dbo.TimeEntry TE
                INNER JOIN dbo.TimeEntryHours AS TEH ON TE.TimeEntryId = TEH.TimeEntryId
                INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
                                            AND TE.PersonId = @PersonId
                                            AND TE.ChargeCodeDate BETWEEN @StartDateLocal
                                                              AND
                                                              @EndDateLocal
                LEFT JOIN dbo.PersonCalendar PC ON PC.PersonId = @PersonId
                                               AND PC.Date = TE.ChargeCodeDate
                                               AND PC.TimeTypeId = CC.TimeTypeId
                                               AND PC.TimeTypeId = @ORTTimeTypeId
                LEFT JOIN dbo.Person AP ON AP.PersonId = PC.ApprovedBy
        WHERE   ( ( @EndDateLocal < '20120401' )
                  OR ( ( @EndDateLocal >= '20120401' )
                       AND CC.ProjectId != 174
                     )
                )

	
	--List of Charge codes with recursive flag.
        SELECT DISTINCT
                ISNULL(CC.TimeEntrySectionId, PTRS.TimeEntrySectionId) AS 'TimeEntrySectionId' ,
                CC.Id AS 'ChargeCodeId' ,
                ISNULL(CC.ClientId, PTRS.ClientId) AS 'ClientId' ,
                C.Name 'ClientName' ,
                ISNULL(CC.ProjectGroupId, PTRS.ProjectGroupId) AS 'GroupId' ,
                PG.Name 'GroupName' ,
                ISNULL(CC.ProjectId, PTRS.ProjectId) AS 'ProjectId' ,
                p.ProjectNumber ,
                P.Name 'ProjectName' ,
                ISNULL(CONVERT(NVARCHAR(1), PTRS.IsRecursive), 0) AS 'IsRecursive' ,
                P.EndDate ,
                P.IsNoteRequired
        FROM    dbo.TimeEntry TE
                INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
                                                AND TE.PersonId = @PersonId
                                                AND TE.ChargeCodeDate BETWEEN @StartDateLocal
                                                              AND
                                                              @EndDateLocal
                FULL JOIN dbo.PersonTimeEntryRecursiveSelection PTRS ON PTRS.ClientId = CC.ClientId
                                                              AND ISNULL(PTRS.ProjectGroupId,
                                                              0) = ISNULL(CC.ProjectGroupId,
                                                              0)
                                                              AND PTRS.ProjectId = CC.ProjectId
                                                              AND StartDate < @EndDateLocal
                                                              AND EndDate > @StartDateLocal
                                                              AND PTRS.PersonId = TE.PersonId
                INNER JOIN dbo.Client C ON C.ClientId = ISNULL(CC.ClientId,
                                                           PTRS.ClientId)
                LEFT JOIN dbo.ProjectGroup PG ON PG.GroupId = ISNULL(CC.ProjectGroupId,
                                                              PTRS.ProjectGroupId)
                INNER JOIN dbo.Project P ON P.ProjectId = ISNULL(CC.ProjectId,
                                                             PTRS.ProjectId)
        WHERE   ( ISNULL(PTRS.PersonId, @PersonId) = @PersonId
                  AND ( CC.Id IS NULL
                        AND PTRS.StartDate < @EndDateLocal
                        AND ISNULL(PTRS.EndDate, @FutureDateLocal) > @StartDateLocal
                      )
                  OR CC.Id IS NOT NULL
                )
                AND ( ( @EndDateLocal < '20120401' )
                      OR ( ( @EndDateLocal >= '20120401' )
                           AND ISNULL(CC.ProjectId, PTRS.ProjectId) != 174
                         )
                    )
        UNION
        SELECT  CC.TimeEntrySectionId AS 'TimeEntrySectionId' ,
                CC.Id AS 'ChargeCodeId' ,
                CC.ClientId AS 'ClientId' ,
                C.Name 'ClientName' ,
                CC.ProjectGroupId AS 'GroupId' ,
                PG.Name 'GroupName' ,
                CC.ProjectId AS 'ProjectId' ,
                p.ProjectNumber ,
                P.Name 'ProjectName' ,
                0 AS 'IsRecursive' ,
                P.EndDate ,
                P.IsNoteRequired
        FROM    dbo.ChargeCode CC
                INNER JOIN dbo.Client C ON C.ClientId = CC.ClientId
                                       AND CC.TimeEntrySectionId = 4 --Administrative Section 
                                       AND (
												(  CC.TimeTypeId in (@HolidayTimeTypeId,@PTOTimeTypeId)
												   AND @IsW2SalaryPerson = 1
												)
												OR 
												(  CC.TimeTypeId in (@SickLeaveTimeTypeId)
												   AND @IsW2HourlyPerson = 1
												)
											)
                INNER JOIN dbo.Project P ON P.ProjectId = CC.ProjectId
                INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId 

	--List of Charge codes with ISPTO and IsHoliday
        SELECT  CC.ProjectId AS 'ProjectId' ,
				CC.TimeTypeId ,
                1 IsPTO ,
                0 IsHoliday ,
                0 IsORT,
				0 IsUnpaid,
				0 IsSickLeave
        FROM    dbo.ChargeCode CC
        WHERE   CC.TimeTypeId = @PTOTimeTypeId AND @IsW2SalaryPerson = 1
        UNION ALL
        SELECT  CC.ProjectId AS 'ProjectId' ,
				CC.TimeTypeId ,
                0 IsPTO ,
                1 IsHoliday ,
                0 IsORT,
				0 IsUnpaid,
				0 IsSickLeave
        FROM    dbo.ChargeCode CC
        WHERE   CC.TimeTypeId = @HolidayTimeTypeId AND @IsW2SalaryPerson = 1
                AND @IsW2SalaryPerson = 1
        UNION ALL
        SELECT  CC.ProjectId AS 'ProjectId' ,
				CC.TimeTypeId ,
				0 IsPTO ,
                0 IsHoliday ,
                1 IsORT,
				0 IsUnpaid,
				0 IsSickLeave
        FROM    dbo.ChargeCode CC
        WHERE   CC.TimeTypeId = @ORTTimeTypeId AND @IsW2SalaryPerson = 1
		UNION ALL
        SELECT  CC.ProjectId AS 'ProjectId' ,
                CC.TimeTypeId ,
				0 IsPTO ,
                0 IsHoliday ,
                0 IsORT,
				1 IsUnpaid,
				0 IsSickLeave
        FROM    dbo.ChargeCode CC
        WHERE   CC.TimeTypeId = @UnpaidTimeTypeId AND  ( @IsW2SalaryPerson = 1 OR @IsW2HourlyPerson = 1)
			UNION ALL
        SELECT  CC.ProjectId AS 'ProjectId' ,
                CC.TimeTypeId ,
				0 IsPTO ,
                0 IsHoliday ,
                0 IsORT,
				0 IsUnpaid,
				1 IsSickLeave
        FROM    dbo.ChargeCode CC
        WHERE   CC.TimeTypeId = @SickLeaveTimeTypeId AND @IsW2HourlyPerson = 1

    END

