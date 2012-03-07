-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Summary By Period.
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

	;WITH PersonDayBillratesByProjects AS
	(
	  SELECT M.ProjectId,
			 C.Date,
			 AVG(ISNULL(MPE.Amount,0)) AS AvgBillRate
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
	  WHERE MP.PersonId = @PersonId 
			AND C.Date BETWEEN @StartDate AND @EndDate
	  GROUP BY M.ProjectId,
			   C.Date
	)
	SELECT  PRO.Name AS ProjectName,
			SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
					 ELSE 0 
				END) AS BillableHours,
			SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
					 ELSE 0 
				END) AS NonBillableHours,
			SUM(ISNULL(PDBR.AvgBillRate,0) * ( CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
													ELSE 0	
													END
											  )
				) AS TotalValue,
			PRO.ProjectNumber,
			C.Name AS  ClentName
	FROM dbo.TimeEntry AS TE 
	JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	JOIN dbo.Client C ON CC.ClientId = C.ClientId
	JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	LEFT JOIN PersonDayBillratesByProjects PDBR ON PDBR.ProjectId = CC.ProjectId 
													AND TE.ChargeCodeDate = PDBR.Date
	WHERE TE.PersonId = @PersonId 
		AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	GROUP BY PRO.ProjectId,
			 PRO.Name,
			 C.Name,
			 PRO.ProjectNumber
END	
	
