-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
(
	@ProjectNumber NVARCHAR(12)
)
AS
BEGIN

	DECLARE @ProjectId INT = NULL, @Today DATETIME

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber 

	IF(@ProjectId IS NOT NULL)
	BEGIN
		SELECT @Today = dbo.GettingPMTime(GETUTCDATE())

   ;WITH PersonBillrates AS
	(
	  SELECT MP.PersonId,
			 AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate,
			 C.Date
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  WHERE  M.ProjectId = @ProjectId 
	  GROUP BY MP.PersonId,C.Date
	)
	,PersonMaxRoleValues AS
	(
	  SELECT MP.PersonId,
			 MAX(ISNULL(PR.RoleValue,0)) AS MaxRoleValue,
			 MIN(CAST(M.IsHourlyAmount AS INT)) IsPersonNotAssignedToFixedProject
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
	  WHERE  M.ProjectId = @ProjectId 
	  GROUP BY MP.PersonId
	)
	,PersonForeCastedHoursUntilToday AS
	(
	  SELECT MP.PersonId,
			 SUM(MPE.HoursPerDay) AS ForecastedHoursUntilToday
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate AND C.DayOff = 0 AND c.Date <= @Today
	  WHERE  M.ProjectId = @ProjectId 
	  GROUP BY MP.PersonId
	)

	SELECT P.PersonId,
		   P.LastName,
		   P.FirstName,
		   ROUND(SUM(CASE WHEN (TEH.IsChargeable = 1 AND TE.ChargeCodeDate <= @Today) THEN TEH.ActualHours ELSE 0 END),2) AS BillableHoursUntilToday,
		   ROUND(SUM(CASE WHEN TEH.IsChargeable = 1  THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
	       ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
		   ROUND(SUM(ISNULL(PersonBillRate.AvgBillRate,0) * ( CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
																	ELSE 0	
																	END
															)
				),2) AS BillableValue,
		   ISNULL(PR.Name,'') AS ProjectRoleName,
		   MIN(ISNULL(PMRV.IsPersonNotAssignedToFixedProject,2))  AS IsPersonNotAssignedToFixedProject,
		   ROUND(MAX(ISNULL(PFH.ForecastedHoursUntilToday,0)),2) AS ForecastedHoursUntilToday
	FROM dbo.TimeEntry AS TE
	INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	FULL  JOIN PersonMaxRoleValues AS PMRV ON PMRV.PersonId = TE.PersonId 
	INNER JOIN dbo.Person AS P ON (P.PersonId = TE.PersonId OR  PMRV.PersonId = P.PersonId) AND p.IsStrawman = 0
	LEFT  JOIN PersonForeCastedHoursUntilToday AS PFH ON PFH.PersonId = P.PersonId  AND PFH.PersonId= PMRV.PersonId 
	LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
	LEFT  JOIN PersonBillrates AS PersonBillRate  ON PersonBillRate.PersonId = P.PersonId AND PersonBillRate.PersonId = PMRV.PersonId AND PersonBillRate.PersonId =PFH.PersonId 
													 AND PersonBillRate.Date = te.ChargeCodeDate 
	GROUP BY P.PersonId,
			 P.LastName,
		     P.FirstName,
			 PR.Name
	
	END
	ELSE
	BEGIN
		RAISERROR('There is no Project exist with this Project Number.', 16, 1)
	END
END

