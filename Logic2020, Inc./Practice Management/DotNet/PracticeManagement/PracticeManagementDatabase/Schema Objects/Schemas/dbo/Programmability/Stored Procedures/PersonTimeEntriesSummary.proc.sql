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

	DECLARE @StartDateLocal DATETIME,
			@EndDateLocal   DATETIME,
			@PersonIdLocal    INT,
			@HolidayTimeType INT ,
			@FutureDate DATETIME


	SELECT @StartDateLocal = CONVERT(DATE,@StartDate), @EndDateLocal = CONVERT(DATE,@EndDate), @PersonIdLocal = @PersonId, @HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate()
	
	;WITH PersonByProjectsBillableTypes AS
	(
	  SELECT M.ProjectId,
			MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
			MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  WHERE MP.PersonId = @PersonIdLocal 
			AND M.StartDate < @EndDateLocal 
			AND @StartDateLocal  < M.ProjectedDeliveryDate
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
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS NonBillableHours
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
			PDBR.MaximumValue
	ORDER BY CC.TimeEntrySectionId,PRO.ProjectNumber
END	
	

