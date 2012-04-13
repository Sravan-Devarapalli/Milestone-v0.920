-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 04-09-2012
-- Description:  Time Entries for a particular period.
/*
Person		Date		Change
----------- ----------- --------------------------------------------------------
Mike Inman	2012/04/10	split first name and last name
						changed output column from status to EmpStatus
						Parameters made optional with default startdate 16 Months in past and end three weeks into future.
						Added CTE to show one row for each day that person is active employee.
						Removed several columns and tables
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
	SET @StartDate =DATEADD(MM,-16,GETDATE())
IF @EndDate IS NULL
	SET @EndDate = DATEADD(ww,3,GETDATE())
SET NOCOUNT ON

/*
 Employee Name	Employee ID	Pay Type	IsOffshore	Status	Account	Account Name	
 Business Unit	Business Unit Name	Project	Project Name	Status	Billing	Phase	
 Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note
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
	--MAKE LIST OF EACH EMPLOYEE AND EACH DAY THAT THEY WERE EMPLOYED AND THREE WEEKS INTO FUTURE.
	,PersonByTime as
	(
	SELECT p.PersonId,
			P.LastName,
			P.FirstName,
			P.EmployeeNumber AS [Employee ID],
			CASE WHEN P.IsOffshore = 1 THEN 'Yes' ELSE 'No' END AS [IsOffshore],
			PerStatus.Name AS  [Status],
			dd.[CalendarDate],
			cast([Year] as char(4)) + ' - ' + 'Week ' + Cast(dd.[WeekOfYear] as char(2))  as [WeekOfYear],
			 cast([Year] as char(4)) + ' - ' + Cast(dd.[MonthName] as varchar(10)) as MonthOfYear,
			dd.[DayOfWeek],
			dd.[QuarterName],
			dd.[Year]
	FROM		dbo.Person P
	INNER JOIN [dbo].[PersonStatusHistory] psh ON P.PersonId = psh.PersonId
	INNER JOIN [dbo].[DateDimension] dd ON dd.[CalendarDate] between psh.StartDate and COALESCE(psh.EndDate, p.[TerminationDate], DATEADD(ww,3,GETDATE()))
	INNER JOIN dbo.PersonStatus AS PerStatus ON PerStatus.PersonStatusId = P.PersonStatusId 	
	WHERE		dd.[CalendarDate] BETWEEN @StartDateLocal AND @EndDateLocal 
	)
	--select * from PersonByTime
	SELECT	
			pbt.PersonId,
			pbt.LastName,
			pbt.FirstName,
			pbt.[Employee ID],
			pbt.[IsOffshore],
			pbt.[CalendarDate],
			pbt.[DayOfWeek],
			pbt.[WeekOfYear],
			pbt.[MonthOfYear],
			pbt.[QuarterName],
			pbt.[Year],
			C.Code AS [Account],
			C.Name AS [Account Name],
			PG.Code AS [Business Unit],
			PG.Name AS [Business Unit Name],
			PRO.ProjectNumber AS [Project],
			PRO.Name AS [Project Name],
			PS.Name AS  [EmpStatus],
			'1' AS [Phase],
			TT.Code AS [Work Type],
			TT.Name AS [Work Type Name],
			TE.ChargeCodeDate AS [Date],
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS [Billable Hours],
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR Pro.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS  [Non-Billable Hours],
			COALESCE(ROUND(SUM(TEH.ActualHours),2),0) AS [Total Hours],
			TE.Note AS [Note]
			FROM    PersonByTime pbt   
			LEFT JOIN  dbo.TimeEntry TE ON pbt.PersonId = te.PersonId 
										AND pbt.CalendarDate = TE.ChargeCodeDate 
			LEFT JOIN  dbo.TimeEntryHours TEH	ON TEH.TimeEntryId = te.TimeEntryId
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
						pbt.[IsOffshore],
						pbt.[CalendarDate],
						pbt.[DayOfWeek],
						pbt.[WeekOfYear],
						pbt.[MonthOfYear],
						pbt.[QuarterName],
						pbt.[Year],
						TE.ChargeCodeDate,
						C.Code,
						C.Name,
						PG.Code,
						PG.Name,
						PRO.ProjectNumber,
						PRO.Name,
						PS.Name,
						TT.Code,
						TT.Name,
						CC.TimeEntrySectionId,
						TE.Note
			ORDER BY Pbt.LastName,CalendarDate
END

