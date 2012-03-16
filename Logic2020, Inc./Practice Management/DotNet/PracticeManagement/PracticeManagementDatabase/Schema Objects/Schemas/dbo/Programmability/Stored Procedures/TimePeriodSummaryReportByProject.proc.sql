-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Thulasiram.P
-- Update Date: 03-15-2012
-- Description:  Time Entries grouped by Project for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByProject]
(
	@StartDate DATETIME,
	@EndDate   DATETIME,
	@ClientIds NVARCHAR(MAX) = NULL,
	@PersonStatusIds NVARCHAR(MAX) = NULL,
	@OrderByCerteria NVARCHAR(20) = 'client' -- client,projectStatus,total
)
AS
BEGIN

	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	DECLARE @GroupByCerteria NVARCHAR(20),@DaysDiff INT 
	SET @DaysDiff = DATEDIFF(dd,@StartDate,@EndDate)
	IF(@DaysDiff <= 7 )
	BEGIN
		SET @GroupByCerteria = 'day'
	END
	ELSE IF(@DaysDiff > 7 AND @DaysDiff <= 31  )
	BEGIN
		SET @GroupByCerteria = 'week'
	END
	ELSE IF(@DaysDiff > 31 AND @DaysDiff <= 365  )
	BEGIN
		SET @GroupByCerteria = 'month'
	END
	ELSE 
	BEGIN
		SET @GroupByCerteria = 'year'
	END

	DECLARE @ClientIdsTable TABLE(ID INT)
	INSERT INTO @ClientIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@ClientIds)

	DECLARE @PersonStatusIdsTable TABLE(ID INT)
	INSERT INTO @PersonStatusIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PersonStatusIds)


SELECT		Data.ClientId,
			C.Name AS ClientName,
			Data.ProjectId,
			P.Name AS ProjectName,
			P.ProjectNumber,
			Data.IsChargeable,
			PS.Name AS ProjectStatusName,
			BillableHours,
			NonBillableHours,
			Data.StartDate,
			GroupByCerteria,
			P.ProjectNumber,
			PS.ProjectStatusId
FROM(
	SELECT  CC.ClientId,
			CC.ProjectId,
			TEH.IsChargeable,
			Round(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
			Round(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
			CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
				WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
				WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
				WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
			END as StartDate,
			@GroupByCerteria 'GroupByCerteria'
	FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND (CC.ClientId IN (SELECT * FROM @ClientIdsTable) OR @ClientIds IS NULL)
	GROUP BY CC.ClientId,
		 	 CC.ProjectId,
			 TEH.IsChargeable,
			CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
				 WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
				 WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
				 WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
			END 
	) Data
	INNER JOIN dbo.Project P ON P.ProjectId = Data.ProjectId
	INNER JOIN dbo.Client C ON C.ClientId = Data.ClientId 
	INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId AND (PS.ProjectStatusId IN (SELECT * FROM @PersonStatusIdsTable) OR @PersonStatusIds IS NULL)

	ORDER BY CASE WHEN @OrderByCerteria = 'client' THEN 2
				  WHEN @OrderByCerteria = 'projectStatus' THEN 7
				  WHEN @OrderByCerteria = 'total' THEN 8
		 	 END
END
	
