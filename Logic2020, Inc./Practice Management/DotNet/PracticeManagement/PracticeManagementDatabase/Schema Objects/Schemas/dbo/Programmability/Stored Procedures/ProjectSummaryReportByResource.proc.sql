-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated by : Sainath.CH
-- Update Date: 04-12-2012
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
	DECLARE @StartDateLocal DATETIME = NULL,
			@EndDateLocal   DATETIME = NULL,
			@ProjectNumberLocal NVARCHAR(12),
			@MilestoneIdLocal   INT = NULL,
			@HolidayTimeType	INT,
			@ProjectId INT = NULL,
			@Today DATE,
			@MilestoneStartDate DATETIME = NULL,
			@MilestoneEndDate DATETIME = NULL

	SET @ProjectNumberLocal = @ProjectNumber
	
	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumberLocal AND @ProjectNumberLocal != 'P999918' --Business Development Project 

	IF(@ProjectId IS NOT NULL)
	BEGIN

		SET @Today = dbo.GettingPMTime(GETUTCDATE())
		SET @MilestoneIdLocal = @MilestoneId
		SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()

		IF(@StartDate IS NOT NULL AND @EndDate IS NOT NULL)
		BEGIN
			SET @StartDateLocal = CONVERT(DATE,@StartDate)
			SET @EndDateLocal = CONVERT(DATE,@EndDate)
		END

		IF(@MilestoneIdLocal IS NOT NULL)
		BEGIN
			SELECT @MilestoneStartDate = M.StartDate,
				   @MilestoneEndDate = M.ProjectedDeliveryDate
			FROM dbo.Milestone AS M
			WHERE M.MilestoneId = @MilestoneIdLocal 
		END
   
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
		  WHERE  M.ProjectId = @ProjectId AND (@MilestoneIdLocal IS NULL OR M.MilestoneId = @MilestoneIdLocal)
		   AND ((@StartDateLocal IS NULL AND @EndDateLocal IS NULL) OR (C.Date BETWEEN  @StartDateLocal AND @EndDateLocal))
		  GROUP BY MP.PersonId
		)
		,PersonForeCastedHours AS
		(
		  SELECT MP.PersonId,
				SUM(CASE WHEN c.Date < @Today THEN  MPE.HoursPerDay ELSE 0 END) AS ForecastedHoursUntilToday,
				SUM(MPE.HoursPerDay) AS ForecastedHours
		  FROM  dbo.MilestonePersonEntry AS MPE 
		  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
		  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate AND C.DayOff = 0  
		  WHERE  M.ProjectId = @ProjectId  AND (@MilestoneIdLocal IS NULL OR M.MilestoneId = @MilestoneIdLocal)
		  AND ((@StartDateLocal IS NULL AND @EndDateLocal IS NULL) OR (C.Date BETWEEN  @StartDateLocal AND @EndDateLocal))
		  GROUP BY MP.PersonId
		)
		,PersonTotalStatusHistory
		AS
		(
			SELECT PSH.PersonId,
					CASE WHEN PSH.StartDate = MinPayStartDate THEN PS.HireDate ELSE PSH.StartDate END AS StartDate,
					ISNULL(PSH.EndDate,dbo.GetFutureDate()) AS EndDate,
					PSH.PersonStatusId
			FROM dbo.PersonStatusHistory PSH 
			LEFT JOIN (
						SELECT PSH.PersonId
									,P.HireDate 
									,MIN(PSH.StartDate) AS MinPayStartDate
						FROM dbo.PersonStatusHistory PSH
						INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId
						WHERE P.IsStrawman = 0
						GROUP BY PSH.PersonId,P.HireDate
						HAVING P.HireDate < MIN(PSH.StartDate)
					) AS PS ON PS.PersonId = PSH.PersonId
		)

		SELECT P.PersonId,
			   P.LastName,
			   P.FirstName,
			   ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND TE.ChargeCodeDate < @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday,
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
											AND ((@MilestoneIdLocal IS NULL) OR (TE.ChargeCodeDate BETWEEN @MilestoneStartDate AND @MilestoneEndDate))
											AND ((@StartDateLocal IS NULL AND @EndDateLocal IS NULL) OR (TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal))
		INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
		INNER JOIN PersonTotalStatusHistory PTSH ON PTSH.PersonId = TE.PersonId 
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate AND PTSH.EndDate
		FULL  JOIN PersonMaxRoleValues AS PMRV ON PMRV.PersonId = TE.PersonId 
		INNER JOIN dbo.Person AS P ON (P.PersonId = TE.PersonId OR  PMRV.PersonId = P.PersonId) AND p.IsStrawman = 0
		LEFT  JOIN PersonForeCastedHours AS PFH ON PFH.PersonId = P.PersonId  AND PFH.PersonId= PMRV.PersonId 
		LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
		WHERE  TE.ChargeCodeDate IS NULL 
				OR 
				(
					TE.ChargeCodeDate <= ISNULL(P.TerminationDate,dbo.GetFutureDate())
					AND (
							CC.timeTypeId != @HolidayTimeType
							OR (CC.timeTypeId = @HolidayTimeType AND PTSH.PersonStatusId = 1 )
						)	
				)
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

