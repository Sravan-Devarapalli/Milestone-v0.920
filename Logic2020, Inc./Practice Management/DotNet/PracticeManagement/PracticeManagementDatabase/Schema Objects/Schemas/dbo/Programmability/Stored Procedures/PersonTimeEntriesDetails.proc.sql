﻿-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Details By Period.
-- Updated by : Sainath.CH
-- Update Date: 04-12-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonTimeEntriesDetails]
    (
      @PersonId INT ,
      @StartDate DATETIME ,
      @EndDate DATETIME
    )
AS 
    BEGIN

        SET NOCOUNT ON;

        DECLARE @StartDateLocal DATETIME ,
            @EndDateLocal DATETIME ,
            @PersonIdLocal INT ,
            @ORTTimeTypeId INT ,
            @HolidayTimeType INT 

        SET @StartDateLocal = CONVERT(DATE, @StartDate)
        SET @EndDateLocal = CONVERT(DATE, @EndDate)
        SET @PersonIdLocal = @PersonId
        SET @ORTTimeTypeId = dbo.GetORTTimeTypeId()
        SET @HolidayTimeType = dbo.GetHolidayTimeTypeId();
        WITH    PersonDayWiseByProjectsBillableTypes
                  AS ( SELECT   M.ProjectId ,
                                C.Date ,
                                MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
                                MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
                       FROM     dbo.MilestonePersonEntry AS MPE
                                INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
                                INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
                                INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
                                                              AND C.Date BETWEEN @StartDateLocal AND @EndDateLocal
                       WHERE    MP.PersonId = @PersonIdLocal
                                AND M.StartDate < @EndDateLocal
                                AND @StartDateLocal < M.ProjectedDeliveryDate
                       GROUP BY M.ProjectId ,
                                C.Date
                     ),
                PersonTotalStatusHistory
                  AS ( SELECT   CASE WHEN PSH.StartDate = MinPayStartDate
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
                                                              AND P.PersonId = @PersonIdLocal
                                            WHERE   P.IsStrawman = 0
                                            GROUP BY PSH.PersonId ,
                                                    P.HireDate
                                            HAVING  P.HireDate < MIN(PSH.StartDate)
                                          ) AS PS ON PS.PersonId = PSH.PersonId
                       WHERE    PSH.PersonId = @PersonIdLocal
                                AND PSH.StartDate < @EndDateLocal
                                AND @StartDateLocal < ISNULL(PSH.EndDate,
                                                             dbo.GetFutureDate())
                     )
            SELECT  CC.TimeEntrySectionId ,
                    C.ClientId ,
                    C.Name AS ClientName ,
                    C.Code AS ClientCode ,
                    BU.Name AS GroupName ,
                    BU.Code AS GroupCode ,
                    PRO.ProjectId ,
                    PRO.Name AS ProjectName ,
                    PRO.ProjectNumber ,
                    ( CASE WHEN ( CC.TimeEntrySectionId <> 1 ) THEN ''
                           ELSE PS.Name
                      END ) AS ProjectStatusName ,
                    TT.TimeTypeId ,
                    TT.Name AS TimeTypeName ,
                    TT.Code AS TimeTypeCode ,
                    TE.ChargeCodeDate ,
                    ( CASE WHEN ( PDBR.MinimumValue IS NULL
                                  OR CC.TimeEntrySectionId <> 1
                                ) THEN ''
                           WHEN ( PDBR.MinimumValue = PDBR.MaximumValue
                                  AND PDBR.MinimumValue = 0
                                ) THEN 'Fixed'
                           WHEN ( PDBR.MinimumValue = PDBR.MaximumValue
                                  AND PDBR.MinimumValue = 1
                                ) THEN 'Hourly'
                           ELSE 'Both'
                      END ) AS BillingType ,
                    ( CASE WHEN ( TT.TimeTypeId = @ORTTimeTypeId
                                  OR TT.TimeTypeId = @HolidayTimeType
                                )
                           THEN TE.Note
                                + dbo.GetApprovedByName(TE.ChargeCodeDate,
                                                        TT.TimeTypeId,
                                                        @PersonIdLocal)
                           ELSE TE.Note
                      END ) AS Note ,
                    ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
                                   THEN TEH.ActualHours
                                   ELSE 0
                              END), 2) AS BillableHours ,
                    ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
                                   THEN TEH.ActualHours
                                   ELSE 0
                              END), 2) AS NonBillableHours
            FROM    dbo.TimeEntry AS TE
                    INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
                    INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
                    INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
                    INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
                    INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
                    INNER JOIN dbo.ProjectStatus PS ON PRO.ProjectStatusId = PS.ProjectStatusId
                    INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
                    INNER JOIN PersonTotalStatusHistory PTSH ON TE.ChargeCodeDate BETWEEN PTSH.StartDate
                                                              AND
                                                              PTSH.EndDate
                    LEFT JOIN PersonDayWiseByProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId
                                                              AND PDBR.Date = TE.ChargeCodeDate
            WHERE   TE.PersonId = @PersonIdLocal
                    AND TE.ChargeCodeDate BETWEEN @StartDateLocal
                                          AND     @EndDateLocal
                    AND ( CC.timeTypeId != @HolidayTimeType
                          OR ( CC.timeTypeId = @HolidayTimeType
                               AND PTSH.PersonStatusId = 1
                             )
                        )
            GROUP BY CC.TimeEntrySectionId ,
                    C.ClientId ,
                    C.Name ,
                    C.Code ,
                    BU.Name ,
                    BU.Code ,
                    PRO.ProjectId ,
                    PRO.Name ,
                    PRO.ProjectNumber ,
                    PS.Name ,
                    TT.TimeTypeId ,
                    TT.Name ,
                    TT.Code ,
                    TE.ChargeCodeDate ,
                    TE.Note ,
                    PDBR.MinimumValue ,
                    PDBR.MaximumValue
            ORDER BY CC.TimeEntrySectionId ,
                    PRO.ProjectNumber ,
                    TE.ChargeCodeDate ,
                    TT.Name
    END	
	

