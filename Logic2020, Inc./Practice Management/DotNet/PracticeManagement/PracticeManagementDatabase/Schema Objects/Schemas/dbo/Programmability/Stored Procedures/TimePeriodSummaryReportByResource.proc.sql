-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Description:  Time Entries grouped by Resource for a particular period.
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryReportByResource]
(
	@StartDate DATETIME,
	@EndDate   DATETIME,
	@SeniorityIds NVARCHAR(MAX) = NULL,
	@OrderByCerteria NVARCHAR(20) = 'name'-- name,seniority,total
)
AS
BEGIN

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

	DECLARE @SeniorityIdsTable TABLE(ID INT)
	INSERT INTO @SeniorityIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@SeniorityIds)


	SELECT	Date.PersonId,
			P.LastName +', ' + P.FirstName AS PersonName,
			IsChargeable,
			S.SeniorityId,
			S.Name,
			StartDate,
			GroupByCerteria,
			TotalHours
	FROM  (
			SELECT  TE.PersonId,
				TEH.IsChargeable,
				Round(SUM(TEH.ActualHours),2) AS TotalHours,
				CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
					WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
					WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
					WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
				END as StartDate,
				@GroupByCerteria 'GroupByCerteria'
				FROM dbo.TimeEntry TE
					INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
				GROUP BY 
						TE.PersonId,
						TEH.IsChargeable,
						CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
							  WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
							  WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
							  WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
						 END
		) Date 
		INNER JOIN dbo.Person P ON P.PersonId = Date.PersonId
		INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
		WHERE (S.SeniorityId in (SELECT * FROM @SeniorityIdsTable) OR @SeniorityIds IS NULL)
		ORDER BY CASE WHEN @OrderByCerteria = 'name' THEN 2
						WHEN @OrderByCerteria = 'seniority' THEN 5
						WHEN @OrderByCerteria = 'total' THEN 6
					END,
					CASE WHEN @OrderByCerteria = 'seniority' THEN 2 END
END
