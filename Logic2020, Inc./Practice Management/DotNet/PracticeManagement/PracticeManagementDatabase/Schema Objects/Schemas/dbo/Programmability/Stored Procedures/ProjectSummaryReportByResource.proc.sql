-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated by : Sainath.CH
-- Update Date: 04-05-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
(
	@ProjectNumber NVARCHAR(12),
	@MilestoneId   INT = NULL,
	@StartDate DATETIME = NULL,
	@EndDate   DATETIME = NULL
)
AS
BEGIN

	IF(@StartDate IS NOT NULL AND @EndDate IS NOT NULL)
	BEGIN
		SET @StartDate = CONVERT(DATE,@StartDate)
		SET @EndDate = CONVERT(DATE,@EndDate)
	END
	
	DECLARE @ProjectId INT = NULL, @Today DATETIME,@MilestoneStartDate DATETIME = NULL,@MilestoneEndDate DATETIME = NULL

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber AND @ProjectNumber != 'P999918' --Business Development Project 



	IF(@MilestoneId IS NOT NULL)
	BEGIN
		SELECT @MilestoneStartDate = M.StartDate,
			   @MilestoneEndDate = M.ProjectedDeliveryDate
		FROM dbo.Milestone AS M
		WHERE M.MilestoneId = @MilestoneId 
	END

	IF(@ProjectId IS NOT NULL)
	BEGIN
		SELECT @Today = dbo.GettingPMTime(GETUTCDATE())

   
	;WITH PersonMaxRoleValues AS
	(
	  SELECT MP.PersonId,
			 MAX(ISNULL(PR.RoleValue,0)) AS MaxRoleValue,
			 MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
			 MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate AND C.DayOff = 0  
	  LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
	  WHERE  M.ProjectId = @ProjectId AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	   AND ((@StartDate IS NULL AND @EndDate IS NULL) OR (C.Date BETWEEN  @StartDate AND @EndDate))
	  GROUP BY MP.PersonId
	)
	,PersonForeCastedHours AS
	(
	  SELECT MP.PersonId,
			 SUM(CASE WHEN c.Date <= @Today THEN  MPE.HoursPerDay ELSE 0 END) AS ForecastedHoursUntilToday,
			 SUM(MPE.HoursPerDay) AS ForecastedHours
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate AND C.DayOff = 0  
	  WHERE  M.ProjectId = @ProjectId  AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	  AND ((@StartDate IS NULL AND @EndDate IS NULL) OR (C.Date BETWEEN  @StartDate AND @EndDate))
	  GROUP BY MP.PersonId
	)

	SELECT P.PersonId,
		   P.LastName,
		   P.FirstName,
		   ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND TE.ChargeCodeDate <= @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday,
		   ROUND(SUM(CASE WHEN TEH.IsChargeable = 1  THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
	       ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
		   ISNULL(PR.Name,'') AS ProjectRoleName,
		   (CASE WHEN (PMRV.MinimumValue IS NULL ) THEN '' 
				WHEN (PMRV.MinimumValue = PMRV.MaximumValue AND PMRV.MinimumValue = 0) THEN 'Fixed'
				WHEN (PMRV.MinimumValue = PMRV.MaximumValue AND PMRV.MinimumValue = 1) THEN 'Hourly'
				ELSE 'Both' END) AS BillingType,
		   ROUND(MAX(ISNULL(PFH.ForecastedHoursUntilToday,0)),2) AS ForecastedHoursUntilToday,
		   ROUND(MAX(ISNULL(PFH.ForecastedHours,0)),2) AS ForecastedHours
	FROM dbo.TimeEntry AS TE
	INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId 
										AND ((@MilestoneId IS NULL) OR (TE.ChargeCodeDate BETWEEN @MilestoneStartDate AND @MilestoneEndDate))
										AND ((@StartDate IS NULL AND @EndDate IS NULL) OR (TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate))
	INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	FULL  JOIN PersonMaxRoleValues AS PMRV ON PMRV.PersonId = TE.PersonId 
	INNER JOIN dbo.Person AS P ON (P.PersonId = TE.PersonId OR  PMRV.PersonId = P.PersonId) AND p.IsStrawman = 0
	LEFT  JOIN PersonForeCastedHours AS PFH ON PFH.PersonId = P.PersonId  AND PFH.PersonId= PMRV.PersonId 
	LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
	GROUP BY P.PersonId,
			 P.LastName,
		     P.FirstName,
			 PR.Name,
			 PMRV.MinimumValue,
			 PMRV.MaximumValue
	
	END
	ELSE
	BEGIN
		RAISERROR('There is no Project with this Project Number.', 16, 1)
	END
END

