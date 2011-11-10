CREATE PROCEDURE [dbo].[TimeEntriesGetByProject]
	@ProjectId	INT,
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL,
	@PersonIds  VARCHAR(MAX) = NULL,
	@MilestoneID INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM Milestone WHERE MilestoneId = @MilestoneId) OR @MilestoneID IS NULL
	BEGIN
	SELECT DISTINCT  TimeEntryId,
			EntryDate,
			ModifiedDate,
			MilestonePersonId,
			ActualHours,
			ForecastedHours,
			TimeTypeId,
			TimeTypeName,
			ModifiedBy,
			Note,
			IsReviewed,
			IsChargeable,
			MilestoneDate,
			ModifiedByFirstName,
			ModifiedByLastName,
			PersonId,
			ObjectFirstName,
			ObjectLastName,
			MilestoneName,
			MilestoneId,
			ProjectId,
			ProjectName,
			ProjectNumber,
			IsCorrect,
			ClientId,
			ClientName,
			IsProjectChargeable
			
	from v_TimeEntries as te
	where te.ProjectId = @ProjectId
		AND te.MilestoneDate BETWEEN ISNULL(@StartDate, te.MilestoneDate) AND ISNULL(@EndDate, te.MilestoneDate)
		AND ((@PersonIds IS NULL) OR (te.PersonId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PersonIds))))
		AND (te.MilestoneId = @MilestoneID OR @MilestoneID IS NULL)
	order by te.PersonId, te.MilestoneDate
	END
	ELSE
	BEGIN
		DECLARE @MileStoneStartDate DATETIME,
				@MileStoneEndDate DATETIME,
				@MilestoneIdLocal INT
		SELECT @MilestoneIdLocal = MilestoneId
		FROM dbo.DefaultMilestoneSetting 
		SELECT @MileStoneStartDate = CONVERT(DATETIME,CONVERT(NVARCHAR,@MilestoneId)) 
		SELECT @MileStoneEndDate = DATEADD(MM,1,@MileStoneStartDate)-1
		
		SELECT      te.TimeEntryId,
					te.EntryDate,
					te.ModifiedDate,
					te.MilestonePersonId,
					te.ActualHours,
					te.ForecastedHours,
					te.TimeTypeId,
					te.TimeTypeName,
					te.ModifiedBy,
					te.Note,
					te.IsReviewed,
					te.IsChargeable,
					te.MilestoneDate,
					te.ModifiedByFirstName,
					te.ModifiedByLastName,
					te.PersonId,
					te.ObjectFirstName,
					te.ObjectLastName,
					te.MilestoneName,
					te.MilestoneId,
					te.ProjectId,
					te.ProjectName,
					te.ProjectNumber,
					te.IsCorrect,
					te.ClientId,
					te.ClientName,
					te.IsProjectChargeable
		FROM v_TimeEntries AS te
		WHERE te.ProjectId = @ProjectId
			AND te.MilestoneDate BETWEEN ISNULL(@StartDate, te.MilestoneDate) and ISNULL(@EndDate, te.MilestoneDate)
			AND te.MilestoneDate BETWEEN @MileStoneStartDate AND @MileStoneEndDate
			AND te.MilestoneId = @MilestoneIdLocal
			AND ((@PersonIds IS NULL) OR (te.PersonId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PersonIds))))
			AND (te.MilestoneId = @MilestoneIdLocal)
		ORDER BY te.PersonId, te.MilestoneDate

	END
END

