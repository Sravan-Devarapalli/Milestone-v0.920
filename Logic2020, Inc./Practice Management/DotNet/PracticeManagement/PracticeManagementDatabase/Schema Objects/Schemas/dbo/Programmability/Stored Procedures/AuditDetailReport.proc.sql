-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 04-09-2012
-- Description:  Time Entries for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[AuditDetailReport]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

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

	SELECT	P.LastName + ' ' + P.FirstName AS [Employee Name],
			P.EmployeeNumber AS [Employee ID],
			TS.Name AS [Pay Type],
			CASE WHEN P.IsOffshore = 1 THEN 'Yes' ELSE 'No' END AS [IsOffshore],
			PerStatus.Name AS  [Status],
			C.Code AS [Account],
			C.Name AS [Account Name],
			PG.Code AS [Business Unit],
			PG.Name AS [Business Unit Name],
			PRO.ProjectNumber AS [Project],
			PRO.Name AS [Project Name],
			PS.Name AS  [Status],
			(CASE WHEN (PDBR.MinimumValue IS NULL OR CC.TimeEntrySectionId <> 1 ) THEN '' 
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 0) THEN 'Fixed'
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 1) THEN 'Hourly'
			ELSE 'Both' END) AS [Billing],
			'1' AS [Phase],
			TT.Code AS [Work Type],
			TT.Name AS [Work Type Name],
			TE.ChargeCodeDate AS [Date],
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS [Billable Hours],
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR Pro.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS  [Non-Billable Hours],
			ROUND(SUM(TEH.ActualHours),2) AS [Total Hours],
			TE.Note AS [Note]
			FROM       dbo.TimeEntry TE 
			INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
												 AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal 
			INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
			INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = Pro.ProjectStatusId  
			INNER JOIN dbo.Client C ON C.ClientId = CC.ClientId
			INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
			INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
			INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
			INNER JOIN dbo.PersonStatus AS PerStatus ON PerStatus.PersonStatusId = P.PersonStatusId  
			LEFT JOIN  dbo.Pay PA ON PA.Person = P.PersonId AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,dbo.GetFutureDate())
			LEFT JOIN  dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
			LEFT JOIN ProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId  AND PDBR.Date = TE.ChargeCodeDate AND TE.PersonId = PDBR.PersonId
			WHERE  TE.ChargeCodeDate <= ISNULL(P.TerminationDate,dbo.GetFutureDate())	
			GROUP BY	P.LastName,
						P.FirstName,
						TE.ChargeCodeDate,
						P.EmployeeNumber,
						TS.Name,
						P.IsOffshore,
						C.Code,
						C.Name,
						PG.Code,
						PG.Name,
						PRO.ProjectNumber,
						PRO.Name,
						PS.Name,
						TT.Code,
						TT.Name,
						PDBR.MinimumValue,
						PDBR.MaximumValue,
						CC.TimeEntrySectionId,
						TE.Note,
						PerStatus.Name
			ORDER BY P.LastName,P.FirstName,C.Name,PG.Name,PRO.Name,TT.Name
END

