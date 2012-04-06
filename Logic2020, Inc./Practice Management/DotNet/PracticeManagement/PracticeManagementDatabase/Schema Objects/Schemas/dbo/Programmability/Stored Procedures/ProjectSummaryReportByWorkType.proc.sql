-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-06-2012
-- Description:  Time Entries grouped by workType for a Project.
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByWorkType]
(
	@ProjectNumber NVARCHAR(12),
	@MilestoneId   INT = NULL,
	@StartDate DATETIME = NULL,
	@EndDate   DATETIME = NULL
)
AS
BEGIN

    IF(@StartDate IS NOT NULL AND @EndDate IS NOT NULL)
	BEGIN
		SET @StartDate = CONVERT(DATE,@StartDate)
		SET @EndDate = CONVERT(DATE,@EndDate)
	END
	
	DECLARE @ProjectId INT = NULL, @Today DATETIME,@MilestoneStartDate DATETIME = NULL,@MilestoneEndDate DATETIME = NULL

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber AND @ProjectNumber != 'P999918' --Business Development Project 

	IF(@MilestoneId IS NOT NULL)
	BEGIN
		SELECT @MilestoneStartDate = M.StartDate,
			   @MilestoneEndDate = M.ProjectedDeliveryDate
		FROM dbo.Milestone AS M
		WHERE M.MilestoneId = @MilestoneId 
	END

	IF(@ProjectId IS NOT NULL)
	BEGIN
		SELECT @Today = dbo.GettingPMTime(GETUTCDATE())

		SELECT  TT.TimeTypeId,
			TT.Name AS TimeTypeName,
	        TT.IsDefault,
			TT.IsInternal,
			TT.IsAdministrative,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 AND @ProjectNumber != 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
			ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 OR @ProjectNumber = 'P031000' THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours,
			CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
				WHEN TT.IsDefault = 1 THEN 'Default'
				WHEN TT.IsInternal = 1 THEN 'Internal'
				ELSE 'Project'
			END AS 'Category'
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId 
										AND ((@MilestoneId IS NULL) OR (TE.ChargeCodeDate BETWEEN @MilestoneStartDate AND @MilestoneEndDate))
										AND ((@StartDate IS NULL AND @EndDate IS NULL) OR (TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate))
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId 
	GROUP BY TT.Name,
			 TT.TimeTypeId,
			 TT.IsDefault,
			 TT.IsInternal,
			 TT.IsAdministrative,
			 CASE WHEN TT.IsAdministrative = 1 THEN 'Administrative'
				  WHEN TT.IsDefault = 1 THEN 'Default'
				  WHEN TT.IsInternal = 1 THEN 'Internal'
				  ELSE 'Project'
			 END
	END
	ELSE
	BEGIN
		RAISERROR('There is no Project with this Project Number.', 16, 1)
	END
END
