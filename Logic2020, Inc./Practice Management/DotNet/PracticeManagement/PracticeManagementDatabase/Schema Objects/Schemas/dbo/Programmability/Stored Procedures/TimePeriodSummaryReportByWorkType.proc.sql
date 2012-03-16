-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Thulasiram.P
-- Update Date: 03-15-2012
-- Description:  Time Entries grouped by workType for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByWorkType]
(
	@StartDate DATETIME,
	@EndDate   DATETIME,
	@TimeTypeCategoryIds NVARCHAR(MAX) = NULL,--work types: (Administrative = 1,Default  = 2,Internal = 3,Project =4)
	@OrderByCerteria NVARCHAR(20) = 'workType'-- workType,category,total
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

	DECLARE @TimeTypeCategoryIdsTable TABLE(WorkTypeName NVARCHAR(30))
	INSERT INTO @TimeTypeCategoryIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@TimeTypeCategoryIds)

	SELECT   TT.TimeTypeId,
			TT.Name AS TimeTypeName,
			TT.IsDefault,
			TT.IsInternal,
			TT.IsAdministrative,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
			CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
				 WHEN TT.IsDefault = 1 THEN 'Default'
				 WHEN TT.IsInternal = 1 THEN 'Internal'
				 ELSE 'Project'
			END AS 'Category',
			CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
				 WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week start date 
				 WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
				 WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
			END as StartDate,
			@GroupByCerteria AS GroupByCerteria
			FROM dbo.TimeEntry TE
				INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
				INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
				INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId 
			WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
					AND ( @TimeTypeCategoryIds IS NULL OR
					((SELECT COUNT(WorkTypeName) FROM @TimeTypeCategoryIdsTable WHERE WorkTypeName = 1) = 1 AND TT.IsAdministrative = 1) OR 
					((SELECT COUNT(WorkTypeName) FROM @TimeTypeCategoryIdsTable WHERE WorkTypeName = 2) = 1 AND TT.IsDefault = 1) OR 
					((SELECT COUNT(WorkTypeName) FROM @TimeTypeCategoryIdsTable WHERE WorkTypeName = 3) = 1 AND TT.IsInternal= 1 AND TT.IsAdministrative = 0) OR
					((SELECT COUNT(WorkTypeName) FROM @TimeTypeCategoryIdsTable WHERE WorkTypeName = 4) = 1 AND TT.IsAdministrative = 0 AND TT.IsDefault = 0 AND TT.IsInternal = 0)
					)
			GROUP BY TT.Name,
					 TT.IsDefault,
					 TT.IsInternal,
					 TT.IsAdministrative,
					 TEH.IsChargeable,
					 TT.TimeTypeId,
					 CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
						  WHEN TT.IsDefault = 1 THEN 'Default'
						  WHEN TT.IsInternal = 1 THEN 'Internal'
	  					  ELSE 'Project'
					  END,
					CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
						 WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week start date 
						 WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
						 WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
					END 
			ORDER BY CASE WHEN @OrderByCerteria = 'workType' THEN 1
						  WHEN @OrderByCerteria = 'category' THEN 7
						  WHEN @OrderByCerteria = 'total' THEN 6
		  			 END
END

