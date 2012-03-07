CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
(
	@ProjectId INT,
	@PersonRoleIds NVARCHAR(MAX) = NULL,
	@OrderByCerteria NVARCHAR(20) = 'resource'-- resource,personrole,total
)
AS
BEGIN

	--Presently @PersonRoleIds is not used as personrole is based on milestone. After the personrole is linked to project we need to include  @PersonRoleIds.
	DECLARE @MinimumTimeEntryDate DATETIME,@MaximumTimeEntryDate DATETIME,@GroupByCerteria NVARCHAR(20),@DaysDiff INT 

	SELECT @MinimumTimeEntryDate = MIN(TE.ChargeCodeDate), 
		   @MaximumTimeEntryDate = MAX(TE.ChargeCodeDate) 
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	WHERE ProjectId = @ProjectId

	SET @DaysDiff = DATEDIFF(dd,@MinimumTimeEntryDate,@MaximumTimeEntryDate)
	IF(@DaysDiff <= 7 )
	BEGIN
		SET @GroupByCerteria = 'day'
	END
	ELSE IF(@DaysDiff > 7 AND @DaysDiff <= 31 )
	BEGIN
		SET @GroupByCerteria = 'week'
	END
	ELSE IF(@DaysDiff > 31 AND @DaysDiff <= 365 )
	BEGIN
		SET @GroupByCerteria = 'month'
	END
	ELSE 
	BEGIN
		SET @GroupByCerteria = 'year'
	END
	
	DECLARE @PersonRoleIdsTable TABLE(ID INT)
	INSERT INTO @PersonRoleIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PersonRoleIds)

	SELECT P.PersonId,
	P.LastName + ', ' + P.FirstName AS PersonName,
	Data.IsChargeable,
	Data.TotalHours,
	Data.StartDate,
	GroupByCerteria

	FROM 
	(
	SELECT TE.PersonId,
	CC.ProjectId,
	TEH.IsChargeable,
	Round(SUM(TEH.ActualHours),2) AS TotalHours,
	CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
			WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
			WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
			WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
	END as StartDate,
	@GroupByCerteria AS GroupByCerteria
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	GROUP BY TE.PersonId,
			 CC.ProjectId,
			TEH.IsChargeable,
			CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
				WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week startdate 
				WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
				WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
			END
			)Data
	INNER JOIN dbo.Person P ON P.PersonId = Data.PersonId 
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = Data.ProjectId 

	ORDER BY CASE WHEN @OrderByCerteria = 'resource' THEN 2
					WHEN @OrderByCerteria = 'total' THEN 4
			END

END
