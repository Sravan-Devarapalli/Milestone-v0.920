-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Details By Period.
-- Updated by : Sainath.CH
-- Update Date: 04-02-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonTimeEntriesDetails]
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

	DECLARE @ORTTimeTypeId		INT

	SET @ORTTimeTypeId = dbo.GetORTTimeTypeId()
	
	;WITH PersonDayWiseByProjectsBillableTypes AS
	(
	  SELECT M.ProjectId,C.Date,
			MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue,
			MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate 
									AND C.Date BETWEEN @StartDate AND @EndDate 
	  WHERE MP.PersonId = @PersonId 
			AND M.StartDate BETWEEN @StartDate AND @EndDate 
			AND M.ProjectedDeliveryDate  BETWEEN @StartDate AND @EndDate 
	  GROUP BY M.ProjectId,C.Date
	)

	  SELECT    C.ClientId,
				C.Name AS  ClientName,
				C.Code AS ClientCode,
				BU.Name AS GroupName,
				BU.Code AS GroupCode,
				PRO.ProjectId,
				PRO.Name AS ProjectName,
				PRO.ProjectNumber,
				(CASE WHEN (CC.TimeEntrySectionId <> 1 ) THEN '' ELSE PS.Name  END ) AS ProjectStatusName,
				TT.TimeTypeId,
				TT.Name  AS TimeTypeName,
				TT.Code AS TimeTypeCode,
				TE.ChargeCodeDate,
				TE.Note,
			(CASE WHEN (PDBR.MinimumValue IS NULL OR CC.TimeEntrySectionId <> 1 ) THEN '' 
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 0) THEN 'Fixed'
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 1) THEN 'Hourly'
			ELSE 'Both' END) AS BillingType,
  		   (CASE  WHEN TT.TimeTypeId = @ORTTimeTypeId THEN TE.Note + dbo.GetApprovedByName(TE.ChargeCodeDate,@ORTTimeTypeId,@PersonId)
				   ELSE TE.Note
				   END) AS Note,
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
	  INNER JOIN dbo.ProjectStatus PS ON PRO.ProjectStatusId = PS.ProjectStatusId
	  INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
	  LEFT JOIN PersonDayWiseByProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId  AND PDBR.Date = TE.ChargeCodeDate
	  WHERE TE.PersonId = @PersonId 
			AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
			AND Pro.ProjectNumber != 'P031000'
	  GROUP BY	CC.TimeEntrySectionId,
				C.ClientId,
				C.Name,
				C.Code,
				BU.Name,
				BU.Code,
				PRO.ProjectId,
				PRO.Name,
				PRO.ProjectNumber,
				PS.Name,
				TT.TimeTypeId,
				TT.Name,
				TT.Code,
				TE.ChargeCodeDate,
				TE.Note,
			(CASE WHEN (PDBR.MinimumValue IS NULL OR CC.TimeEntrySectionId <> 1 ) THEN '' 
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 0) THEN 'Fixed'
			WHEN (PDBR.MinimumValue = PDBR.MaximumValue AND PDBR.MinimumValue = 1) THEN 'Hourly'
			ELSE 'Both' END)
	  ORDER BY  CC.TimeEntrySectionId,PRO.ProjectNumber,TE.ChargeCodeDate,TT.Name
END	
	

