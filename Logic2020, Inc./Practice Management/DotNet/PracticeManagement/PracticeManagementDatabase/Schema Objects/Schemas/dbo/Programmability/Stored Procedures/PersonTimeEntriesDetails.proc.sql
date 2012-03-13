-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Details By Period.
-- Updated By : Sainath CH
-- Modified Date : 03-13-2012
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

	  SELECT C.ClientId,
			 PRO.ProjectId,
			 PRO.Name AS ProjectName,
			 PRO.ProjectNumber,
			 C.Name AS  ClientName,
			 TE.ChargeCodeDate,
			 TT.Name AS TimeTypeName,
			 TE.Note,
			 ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours 
					  ELSE 0 
				  END),2) AS BillableHours,
			 ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours 
				      ELSE 0 
				 END),2) AS NonBillableHours
	  FROM dbo.TimeEntry AS TE 
	  JOIN dbo.TimeEntryHours AS TEH  ON TEH.TimeEntryId = TE.TimeEntryId 
	  JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
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
				TT.Name,
				TE.Note
	  ORDER BY  Pro.Name,TE.ChargeCodeDate,TT.Name
END	
	

