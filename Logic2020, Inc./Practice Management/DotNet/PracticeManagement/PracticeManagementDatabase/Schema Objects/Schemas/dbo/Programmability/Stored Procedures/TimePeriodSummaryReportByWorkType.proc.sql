-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-06-2012
-- Description:  Time Entries grouped by workType for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByWorkType]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

    SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	SELECT  TT.TimeTypeId,
			TT.Name AS TimeTypeName,
			TT.IsDefault,
			TT.IsInternal,
			TT.IsAdministrative,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND Pro.ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR Pro.ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
			CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
				 WHEN TT.IsDefault = 1 THEN 'Default'
				 WHEN TT.IsInternal = 1 THEN 'Internal'
				 ELSE 'Project'
			END AS 'Category'
	FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
		INNER JOIN dbo.Project Pro ON CC.ProjectId = Pro.ProjectId
		INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId 
	WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
	GROUP BY TT.TimeTypeId,
		 	TT.Name,
			TT.IsDefault,
			TT.IsInternal,
			TT.IsAdministrative,
			CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
				WHEN TT.IsDefault = 1 THEN 'Default'
				WHEN TT.IsInternal = 1 THEN 'Internal'
	  			ELSE 'Project'
			END
	ORDER BY TT.Name
	
END

