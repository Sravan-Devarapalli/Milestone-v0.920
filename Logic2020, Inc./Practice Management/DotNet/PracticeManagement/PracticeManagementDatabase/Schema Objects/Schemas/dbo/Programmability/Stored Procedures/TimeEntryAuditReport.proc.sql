-- =========================================================================
-- Author:      ThulasiRam.P
-- Create date: 05-07-2012
-- Description: Time Entries for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimeEntryAuditReport]
    (
      @StartDate DATETIME = NULL ,
      @EndDate DATETIME = NULL
    )
AS 
    BEGIN

        SET NOCOUNT ON;

				/*
				Person Name	Status	Pay Type	Affected Date	Modified Date	Account Name	Business Unit	Project	Project Name
				Phase	Work Type	B/NB	Original	New	Net Change	Note
				*/

        DECLARE @StartDateLocal DATETIME ,
            @EndDateLocal DATETIME,
		    @HolidayTimeType INT ,
            @ORTTimeTypeId INT

        SET @StartDateLocal = CONVERT(DATE, @StartDate)
        SET @EndDateLocal = CONVERT(DATE, @EndDate)
		SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()
        SET @ORTTimeTypeId = dbo.GetORTTimeTypeId();

		  WITH    EffectedPersons
                  AS ( 
                       SELECT  Distinct TH.PersonId
                       FROM     dbo.TimeEntryHistory AS TH
                       WHERE    TH.ModifiedDate > @EndDateLocal
                                AND TH.OldHours <> TH.ActualHours
                                AND TH.ChargeCodeDate BETWEEN @StartDateLocal
                                                      AND     @EndDateLocal
                     )
            SELECT  P.PersonId ,
			        P.EmployeeNumber,
                    P.LastName ,
                    P.FirstName ,
                    P.PersonStatusId ,
                    PS.Name AS PersonStatusName,
                    TSC.TimescaleId AS Timescale ,
                    TSC.Name AS TimescaleName
            FROM    EffectedPersons AS EP
                    INNER JOIN dbo.Person AS P ON EP.PersonId = P.PersonId
                    INNER JOIN dbo.PersonStatus AS PS ON P.PersonStatusId = PS.PersonStatusId
                    LEFT JOIN dbo.Timescale AS TSC ON TSC.TimescaleId = dbo.GetCurrentPayType(P.PersonId);


        WITH    TimeEntriesHistory
                  AS ( 
                       SELECT   TH.PersonId ,
                                TH.ChargeCodeDate ,
                                TH.ModifiedDate ,
                                TH.ChargeCodeId ,
                                TH.IsChargeable ,
                                TH.OldHours AS OriginalHours ,
                                TH.ActualHours ,
                                TH.Note
                       FROM     dbo.TimeEntryHistory AS TH
                       WHERE    TH.ModifiedDate > @EndDateLocal
                                AND TH.OldHours <> TH.ActualHours
                                AND TH.ChargeCodeDate BETWEEN @StartDateLocal
                                                      AND     @EndDateLocal
                     )
            SELECT  TH.PersonId ,
                    PROJ.ProjectId ,
                    PROJ.Name AS ProjectName ,
                    PROJ.ProjectNumber ,
                    CLNT.ClientId ,
                    CLNT.Name AS ClientName ,
                    Pg.GroupId ,
                    Pg.Name AS GroupName ,
                    TT.TimeTypeId ,
                    TT.Name AS TimeTypeName ,
                    TH.ChargeCodeDate ,
                    TH.ModifiedDate ,
                    TH.ChargeCodeId ,
                    TH.IsChargeable ,
                    TH.OriginalHours ,
                    TH.ActualHours ,
                    ( CASE WHEN ( TT.TimeTypeId = @ORTTimeTypeId
                                        OR TT.TimeTypeId = @HolidayTimeType
                                    )
                                THEN TH.Note
                                    + dbo.GetApprovedByName(TH.ChargeCodeDate,
                                                            TT.TimeTypeId,
                                                            TH.PersonId)
                                ELSE TH.Note
                            END ) AS Note ,
                    1 AS Phase,
					CC.TimeEntrySectionId
            FROM    TimeEntriesHistory AS TH
                    INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TH.ChargeCodeId
                    INNER JOIN dbo.Project AS PROJ ON PROJ.ProjectId = CC.ProjectId
                    INNER JOIN dbo.Client AS CLNT ON CC.ClientId = CLNT.ClientId
                    INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
                    INNER JOIN dbo.TimeType AS TT ON TT.TimeTypeId = CC.TimeTypeId
			ORDER BY ChargeCodeDate
    END

