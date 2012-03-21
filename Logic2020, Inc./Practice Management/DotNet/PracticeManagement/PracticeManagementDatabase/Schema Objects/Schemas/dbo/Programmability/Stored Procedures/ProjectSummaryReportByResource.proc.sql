-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- Updated by : Thulasiram.P
-- Update Date: 03-21-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
(
	@ProjectNumber NVARCHAR(12)
)
AS
BEGIN

	DECLARE @ProjectId INT 

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber 

	;WITH PersonBillrates AS
	(
	  SELECT MP.PersonId,
			 AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate,
			 C.Date
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId 
	  WHERE  M.ProjectId = @ProjectId AND P.DefaultPractice <> 4 /* Administration */
	  GROUP BY MP.PersonId,C.Date
	)
	,PersonMaxRoleValues AS
	(
	  SELECT MP.PersonId,
			 MAX(ISNULL(PR.RoleValue,0)) AS MaxRoleValue
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId 
	  LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
	  WHERE  M.ProjectId = @ProjectId  AND P.DefaultPractice <> 4 /* Administration */
	  GROUP BY MP.PersonId
	)

	SELECT P.PersonId,
		   P.LastName,
		   P.FirstName,
		   ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
	       ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
		   ROUND(SUM(ISNULL(PersonBillRate.AvgBillRate,0) * ( CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
																	ELSE 0	
																	END
															)
				),2) AS BillableValue,
		   ISNULL(PR.Name,'') AS ProjectRoleName
	FROM dbo.TimeEntry AS TE
	INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId 
	LEFT  JOIN PersonBillrates AS PersonBillRate  ON PersonBillRate.PersonId = P.PersonId AND PersonBillRate.Date = te.ChargeCodeDate AND P.DefaultPractice <> 4 /* Administration */
	LEFT  JOIN PersonMaxRoleValues AS PMRV ON PMRV.PersonId = P.PersonId AND PersonBillRate.PersonId = PMRV.PersonId 
	LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
	GROUP BY P.PersonId,
			 P.LastName,
		     P.FirstName,
			 PR.Name
	ORDER BY P.LastName,
			 P.FirstName
	

END

