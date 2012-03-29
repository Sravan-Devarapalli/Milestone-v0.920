-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Summary By Period.
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
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

	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	;WITH PersonDayBillratesByProjects AS
	(
	  SELECT M.ProjectId,
			 C.Date,
			 AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate,
			 MIN(CAST(M.IsHourlyAmount AS INT)) IsPersonNotAssignedToFixedProject --if return 0 then fixed Amount else not fixed Amount
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  WHERE MP.PersonId = @PersonId 
			AND C.Date BETWEEN @StartDate AND @EndDate
	  GROUP BY M.ProjectId,
			   C.Date
	)
	SELECT  PRO.ProjectId,
			PRO.Name AS ProjectName, 
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS BillableHours,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
					 ELSE 0 
				END),2) AS NonBillableHours,
			ROUND(SUM(ISNULL(PDBR.AvgBillRate,0) * ( CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
													ELSE 0	
													END
											  )
				),2) AS BillableValue,
			MIN(ISNULL(PDBR.IsPersonNotAssignedToFixedProject,2))  AS IsPersonNotAssignedToFixedProject, --if return 0 then fixed Amount else if return 1 not fixed Amount else if return 2 not fixed
			PRO.ProjectNumber,
			C.Name AS  ClientName,
			C.Code AS ClientCode,
			BU.Name AS GroupName,
			CC.TimeEntrySectionId,
			BU.Code AS GroupCode
	FROM dbo.TimeEntry AS TE 
	INNER JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
	INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
	INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	LEFT JOIN PersonDayBillratesByProjects PDBR ON PDBR.ProjectId = CC.ProjectId 
							AND TE.ChargeCodeDate = PDBR.Date
	WHERE TE.PersonId = @PersonId 
		AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	GROUP BY PRO.ProjectId,
			 PRO.Name,
			 C.Name,
			 PRO.ProjectNumber,
			 BU.Name,
			 CC.TimeEntrySectionId,
			 C.Code,
			 BU.Code
	ORDER BY CC.TimeEntrySectionId,PRO.ProjectNumber
END	
	

