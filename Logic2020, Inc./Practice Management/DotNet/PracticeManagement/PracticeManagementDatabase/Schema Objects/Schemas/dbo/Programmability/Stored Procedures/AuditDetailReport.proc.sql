-- =========================================================================
-- Author:           ThulasiRam.P
-- Create date: 04-09-2012
-- Description:  Time Entries for a particular period.
/*
Person        Date          Change
----------- ----------- --------------------------------------------------------
Mike Inman    2012/04/10    split first name and last name
                                         changed output column from status to EmpStatus
                                         Parameters made optional with default startdate 16 Months in past and end three weeks into future.
                                         Added CTE to show one row for each day that person is active employee.
                                         Removed several columns and tables
Mike Inman    2012/04/12    Add anouther CTE so that I could add a weekly and daily total Hours columns per new business requirement
                                         Added summary Columns; Project Hours, DayHours, and WeekHours
Mike Inman    2012/04/19  Added note column as [Comment]
*/
-- =========================================================================
CREATE PROCEDURE [dbo].[AuditDetailReport]
(
       @StartDate DATETIME = NULL,
       @EndDate   DATETIME = NULL
)
AS
BEGIN
-- set default startdate
IF @StartDate IS NULL
       SET @StartDate =DATEADD(MM,-4,GETDATE())
IF @EndDate IS NULL
       SET @EndDate = DATEADD(ww,3,GETDATE())
SET NOCOUNT ON

/*
Employee Name       Employee ID   Pay Type      IsOffshore    Status Account       Account Name  
Business Unit       Business Unit Name   Project       Project Name  Status Billing       Phase  
Work Type    Work Type Name       Date   Billable Hours       Non-Billable Hours   ProjectHours  Note
*/

DECLARE @StartDateLocal DATETIME,
           @EndDateLocal   DATETIME

       SET @StartDateLocal = CONVERT(DATE,@StartDate)
       SET @EndDateLocal = CONVERT(DATE,@EndDate)

       DECLARE @NOW DATE
       SET @NOW = dbo.GettingPMTime(GETUTCDATE())


       ;WITH ProjectsBillableTypes AS
       (
         SELECT M.ProjectId,
                MP.PersonId,
                     C.Date,
                     MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
                     MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
         FROM  dbo.MilestonePersonEntry AS MPE 
         INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
         INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
         INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate 
                                                              AND C.Date BETWEEN @StartDate AND @EndDate 
         WHERE M.StartDate < @EndDate 
                     AND @StartDate  < M.ProjectedDeliveryDate
         GROUP BY M.ProjectId,C.Date,MP.PersonId
       )
       --MAKE LIST OF EACH EMPLOYEE AND EACH DAY THAT THEY WERE EMPLOYED AND THREE WEEKS INTO FUTURE BY PAYROLL TYPE.
       ,PersonByTime as
       (
       SELECT p.PersonId,
                     P.LastName,
                     P.FirstName,
                     P.EmployeeNumber AS [Employee ID],
                     p.[PaychexID],
                     CASE WHEN P.IsOffshore = 1 THEN 'Yes' ELSE 'No' END AS [IsOffshore],
                     PerStatus.Name AS  [Status],
                     dd.[CalendarDate],
                     CASE 
                           WHEN dd.[WeekOfYear] < 10 
                           THEN CAST([Year] as char(4)) + ' - ' + 'Week 0' + Cast(dd.[WeekOfYear] as char(2)) 
                           ELSE CAST([Year] as char(4)) + ' - ' + 'Week ' + Cast(dd.[WeekOfYear] as char(2)) 
                     END as [WeekOfYear],
                     CAST([Year] as char(4)) + ' - ' + Cast(dd.[MonthName] as varchar(10)) as MonthOfYear,
                     dd.[WeekOfYear] as WeekId,
                     CONVERT(Char(8), dd.[CalendarDate], 112) as DateId,
                     dd.[DayOfWeek],
                     dd.[QuarterName],
                     dd.[Year],
                     pay.[TimescaleName] as PayType
       FROM          dbo.Person P
       INNER JOIN [dbo].[PersonStatusHistory] AS psh ON P.PersonId = psh.PersonId
       INNER JOIN [dbo].[DateDimension] AS dd ON dd.[CalendarDate] between psh.StartDate and COALESCE(psh.EndDate, p.[TerminationDate], DATEADD(ww,3,GETDATE()))
       INNER JOIN dbo.PersonStatus AS PerStatus ON PerStatus.PersonStatusId = P.PersonStatusId 
       INNER JOIN dbo.v_Pay AS pay ON p.personId = pay.personID AND dd.CalendarDate between pay.[StartDate] and COALESCE(pay.EndDate, DATEADD(ww,3,GETDATE()))
       WHERE         dd.[CalendarDate] BETWEEN @StartDateLocal AND @EndDateLocal 
       )
       ,ProjectHours as
       (
       SELECT 
                     pbt.PersonId,
                     pbt.LastName,
                     pbt.FirstName,
                     pbt.[Employee ID],
                     pbt.[PaychexID],
                     pbt.[IsOffshore],
                     pbt.[CalendarDate],
                     pbt.[DayOfWeek],
                     pbt.[WeekOfYear],
                     pbt.[MonthOfYear],
                     pbt.[QuarterName],
                     pbt.[Year],
                     pbt.WeekId,
                     pbt.DateId,
                     pbt.PayType,
                     C.Code AS [Account],
                     C.Name AS [Account Name],
                     PG.Code AS [Business Unit],
                     PG.Name AS [Business Unit Name],
                     PRO.ProjectNumber AS [Project],
                     PRO.Name AS [Project Name],
                     pbt.[Status] AS  [EmpStatus],
                     '1' AS [Phase],
                     TT.Code AS [Work Type],
                     TT.Name AS [Work Type Name],
                     ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS [Billable Hours],
                     ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR Pro.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS  [Non-Billable Hours],
                     COALESCE(ROUND(SUM(TEH.ActualHours),2),0) AS [ProjectHours],
                     TE.Note AS [Comment]
                     FROM    PersonByTime pbt   
                     LEFT JOIN  dbo.TimeEntry TE ON pbt.PersonId = te.PersonId 
                                                                     AND pbt.CalendarDate = TE.ChargeCodeDate 
                     LEFT JOIN  dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
                     LEFT JOIN  dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
                     LEFT JOIN  dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
                     LEFT JOIN  dbo.ProjectStatus AS PS ON PS.ProjectStatusId = Pro.ProjectStatusId  
                     LEFT JOIN dbo.Client C ON C.ClientId = CC.ClientId
                     LEFT JOIN  dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
                     LEFT JOIN  dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
                     WHERE  1=1
                     GROUP BY      
                                         pbt.PersonId,
                                         pbt.LastName,
                                         pbt.FirstName,
                                         pbt.[Employee ID],
                                         pbt.[PaychexID],
                                         pbt.[IsOffshore],
                                         pbt.[CalendarDate],
                                         pbt.[DayOfWeek],
                                         pbt.[WeekOfYear],
                                         pbt.[MonthOfYear],
                                         pbt.[QuarterName],
                                         pbt.[Year],
                                         pbt.WeekId,
                                         pbt.DateId,
                                         pbt.PayType,
                                         C.Code,
                                         C.Name,
                                         PG.Code,
                                         PG.Name,
                                         PRO.ProjectNumber,
                                         PRO.Name,
                                         pbt.[Status],
                                         TT.Code,
                                         TT.Name,
                                         CC.TimeEntrySectionId,
                                         TE.Note
       )
       ,DayHours as (
       SELECT PersonId
                     ,DateId
                     ,SUM(ProjectHours) as DayHours
       FROM          ProjectHours
       GROUP BY PersonId
                     ,DateId
       )
       ,WeekHours as (
       SELECT PersonId
                     ,WeekId
                     ,[Year]
                     ,SUM(ProjectHours) as WeekHours
       FROM          ProjectHours
       GROUP BY PersonId
                     ,WeekId
                     ,[Year]
       )

       SELECT 
                     Project.PersonId,
                     Project.LastName,
                     Project.FirstName,
                     Project.[Employee ID],
                     Project.[PaychexID],
                     Project.[IsOffshore],
                     Project.[CalendarDate],
                     Project.[DayOfWeek],
                     Project.[WeekOfYear],
                     Project.[MonthOfYear],
                     Project.[QuarterName],
                     Project.[Year],
                     Project.[Account],
                     Project.[Account Name],
                     Project.[Business Unit],
                     Project.[Business Unit Name],
                     Project.[Project],
                     Project.[Project Name],
                     Project.[EmpStatus],
                     Project.[Phase],
                     Project.[Work Type],
                     Project.[Work Type Name],
                     Project.[Billable Hours],
                     Project.[Non-Billable Hours],
                     Project.[ProjectHours],
                     Project.[Comment],
                     Project.[PayType],
                     DayHours.DayHours,
                     WeekHours.WeekHours
       FROM   ProjectHours  Project
       INNER JOIN DayHours        ON     Project.PersonId = DayHours.PersonId
                                                AND Project.DateId = DayHours.DateId     
       INNER JOIN WeekHours ON     Project.PersonId = WeekHours.PersonId
                                                AND    Project.WeekId = WeekHours.WeekId
                                                AND    Project.[year] = WeekHours.[Year]
       ORDER BY      Project.PersonId,
                           Project.[CalendarDate]
END

