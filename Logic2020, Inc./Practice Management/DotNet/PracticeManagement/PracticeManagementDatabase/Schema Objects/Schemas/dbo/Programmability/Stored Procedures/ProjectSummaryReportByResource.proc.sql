-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResource]
(
	@ProjectNumber NVARCHAR(12),
	@PersonRoleIds NVARCHAR(MAX) = NULL,
	@OrderByCerteria NVARCHAR(20) = 'resource'-- resource,person role,total
)
AS
BEGIN

	DECLARE @ProjectId INT 

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber 

	--Presently @PersonRoleIds is not used as person role is based on milestone. After the person role is linked to project we need to include  @PersonRoleIds.
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
	ELSE IF(@DaysDiff > 31 AND @DaysDiff <= 366 )
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
	P.LastName,
	P.FirstName,
	Data.BillableHours,
	Data.NonBillableHours,
	Data.StartDate,
	GroupByCerteria

	FROM 
	(
	SELECT TE.PersonId,
	CC.ProjectId,
	Round(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
	Round(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
	CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
			WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week start date 
			WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
			WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
	END as StartDate,
	@GroupByCerteria AS GroupByCerteria
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	GROUP BY TE.PersonId,
			 CC.ProjectId,
			CASE WHEN @GroupByCerteria = 'day' THEN TE.ChargeCodeDate -- date
				WHEN @GroupByCerteria = 'week' THEN (TE.ChargeCodeDate - (DATEPART(dw,TE.ChargeCodeDate) -1 ))  --week start date 
				WHEN @GroupByCerteria = 'month' THEN TE.ChargeCodeDate - (DAY(TE.ChargeCodeDate)-1)  -- month StartDate
				WHEN @GroupByCerteria = 'year' THEN TE.ChargeCodeDate - (DATEPART(DAYOFYEAR,TE.ChargeCodeDate)-1)-- year StartDate
			END
			)Data
	INNER JOIN dbo.Person P ON P.PersonId = Data.PersonId 
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = Data.ProjectId 

	ORDER BY CASE WHEN @OrderByCerteria = 'resource' THEN 2
					WHEN @OrderByCerteria = 'total' THEN BillableHours+NonBillableHours
			END

END

