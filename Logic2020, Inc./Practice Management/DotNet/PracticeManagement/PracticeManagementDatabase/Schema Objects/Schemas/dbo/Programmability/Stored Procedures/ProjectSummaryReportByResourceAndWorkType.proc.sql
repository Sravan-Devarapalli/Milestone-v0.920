-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Description:  Time Entries grouped by workType for a Project.
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResourceAndWorkType]
(
	@ProjectId INT,
	@OrderByCerteria NVARCHAR(20) ='resource'-- resource,personrole,total
)
AS
BEGIN

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

	SELECT  P.PersonId,
			P.LastName +', ' + P.FirstName As PersonName,
			TEH.IsChargeable,
			Round(SUM(TEH.ActualHours),2) AS TotalHours,
			CASE WHEN TT.IsAdministrative = 1 THEN 'Adminstrative'
							WHEN TT.IsDefault = 1 THEN 'Default'
							WHEN TT.IsInternal = 1 THEN 'Internal'
							ELSE 'Project'
					END AS Category
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
	INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId 
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = CC.ProjectId 
	INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId 
	WHERE Pro.ProjectId = @ProjectId
	GROUP BY P.PersonId,
			 P.LastName,
			 P.FirstName,
			 TEH.IsChargeable,
			 TT.Name,
			 CASE WHEN TT.IsAdministrative = 1 THEN 'Adminstrative'
			 	  WHEN TT.IsDefault = 1 THEN 'Default'
				  WHEN TT.IsInternal = 1 THEN 'Internal'
				  ELSE 'Project'
			 END
	ORDER BY CASE WHEN @OrderByCerteria = 'resource' THEN 2
				  WHEN @OrderByCerteria = 'personrole' THEN 5
				  WHEN @OrderByCerteria = 'total' THEN 4
			 END
END
