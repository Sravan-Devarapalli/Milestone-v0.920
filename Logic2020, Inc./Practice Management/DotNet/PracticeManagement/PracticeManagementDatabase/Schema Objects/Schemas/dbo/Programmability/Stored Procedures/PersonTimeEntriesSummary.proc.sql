-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Summary By Period.
-- Updated by : ThulasiRam.P
-- Update Date: 06-05-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonTimeEntriesSummary]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Today DATE ,
	        @StartDateLocal DATETIME,
			@EndDateLocal   DATETIME,
			@PersonIdLocal    INT,
			@HolidayTimeType INT ,
			@FutureDate DATETIME


	SELECT @StartDateLocal = CONVERT(DATE,@StartDate), @Today = dbo.GettingPMTime(GETUTCDATE()), @EndDateLocal = CONVERT(DATE,@EndDate), @PersonIdLocal = @PersonId, @HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate()
	
	;WITH PersonByProjectsBillableTypes AS
	(
	  SELECT M.ProjectId,
			MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
			MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue,			
			SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ProjectedHours,
			SUM(CASE WHEN PC.Date < @Today THEN (dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay))
					ELSE 0
				END)  AS ProjectedHoursUntilToday
	  FROM     dbo.MilestonePersonEntry AS MPE
		INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
		INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
									AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate
									AND PC.Date BETWEEN @StartDateLocal AND @EndDateLocal
	  WHERE MP.PersonId = @PersonIdLocal 			
	  GROUP BY M.ProjectId
	)

	SELECT  CC.TimeEntrySectionId,
			C.Name AS  ClientName,
			C.Code AS ClientCode,
			BU.Name AS GroupName,
			BU.Code AS GroupCode,
			PRO.ProjectId,
			PRO.Name AS ProjectName, 
			PRO.ProjectNumber,
			(CASE WHEN (CC.TimeEntrySectionId <> 1 ) THEN '' ELSE PS.Name  END ) AS ProjectStatusName,
			(CASE WHEN (PDBR.MinimumValue IS NULL OR CC.TimeEntrySectionId <> 1 ) THEN '' 
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 0) THEN 'Fixed'
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 1) THEN 'Hourly'
			ELSE 'Both' END) AS BillingType,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS BillableHours,
		    ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND TE.ChargeCodeDate < @Today THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS BillableHoursUntilToday,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS NonBillableHours,
			ROUND(PDBR.ProjectedHours,2) AS ProjectedHours,
			ROUND(PDBR.ProjectedHoursUntilToday,2) AS ProjectedHoursUntilToday
	FROM dbo.TimeEntry AS TE 
	INNER JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
	INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
	INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = PRO.ProjectStatusId
	INNER JOIN dbo.PersonStatusHistory AS PTSH ON TE.ChargeCodeDate BETWEEN PTSH.StartDate AND ISNULL(PTSH.EndDate,@FutureDate) AND PTSH.PersonId = TE.PersonId
	LEFT JOIN PersonByProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId 
	WHERE TE.PersonId = @PersonIdLocal 
		AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
		AND (
				CC.timeTypeId != @HolidayTimeType
				OR (CC.timeTypeId = @HolidayTimeType AND PTSH.PersonStatusId IN (1,5) )
			)	
	GROUP BY CC.TimeEntrySectionId,
			C.Name,
			C.Code,
			BU.Name,
			BU.Code,
			PRO.ProjectId,
			PRO.Name,
			PRO.ProjectNumber, 
			CC.TimeEntrySectionId,
			PS.Name,
			PDBR.MinimumValue,
			PDBR.MaximumValue,
			PDBR.ProjectedHours,
			PDBR.ProjectedHoursUntilToday
	ORDER BY CC.TimeEntrySectionId,PRO.ProjectNumber
END	

