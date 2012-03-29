-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Details By Period.
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
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
	

	  SELECT C.Code AS ClientCode,
			 C.ClientId,
			 PRO.ProjectId,
			 PRO.Name AS ProjectName,
			 PRO.ProjectNumber,
			 C.Name AS  ClientName,
			 TE.ChargeCodeDate,
			 TT.Name AS TimeTypeName,
			 TT.TimeTypeId,
			 (CASE  WHEN TT.TimeTypeId = @ORTTimeTypeId THEN TE.Note + dbo.GetApprovedByName(TE.ChargeCodeDate,@ORTTimeTypeId,@PersonId)
				   ELSE TE.Note
				   END) AS Note
			 ,
			 ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
					  ELSE 0 
				  END),2) AS BillableHours,
			 ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
				      ELSE 0 
				 END),2) AS NonBillableHours,
			BU.Name AS GroupName,
			BU.Code AS GroupCode,
			TT.Code AS TimeTypeCode
	  FROM dbo.TimeEntry AS TE 
	  JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	  JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	  INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
	  JOIN dbo.Client C ON CC.ClientId = C.ClientId
	  JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	  JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
	  WHERE TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	  GROUP BY	C.ClientId,
				PRO.ProjectId,
				PRO.Name,
				PRO.ProjectNumber,
				C.Name,
				TE.ChargeCodeDate,
				TT.TimeTypeId,
				TT.Name,
				TE.Note,
				BU.Name,
				BU.Code,
				C.Code,
				TT.Code,
				CC.TimeEntrySectionId
	  ORDER BY  CC.TimeEntrySectionId,PRO.ProjectNumber,TE.ChargeCodeDate,TT.Name
END	
	

