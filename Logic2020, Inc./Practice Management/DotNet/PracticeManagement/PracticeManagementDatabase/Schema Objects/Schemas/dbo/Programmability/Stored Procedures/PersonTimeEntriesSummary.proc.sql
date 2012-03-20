-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Summary By Period.
-- Updated By : Sainath CH
-- Modified Date : 03-20-2012
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
			PRO.ProjectNumber,
			C.Name AS  ClientName
	FROM dbo.TimeEntry AS TE 
	INNER JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
	INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	LEFT JOIN PersonDayBillratesByProjects PDBR ON PDBR.ProjectId = CC.ProjectId 
							AND TE.ChargeCodeDate = PDBR.Date
	WHERE TE.PersonId = @PersonId 
		AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	GROUP BY PRO.ProjectId,
			 PRO.Name,
			 C.Name,
			 PRO.ProjectNumber
	ORDER BY PRO.Name
END	
	

