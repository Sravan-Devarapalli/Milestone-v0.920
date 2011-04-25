CREATE PROCEDURE [dbo].[CheckTimeEntriesForMilestonePerson]
(
	@MilestonePersonId INT,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL,
	@CheckStartDateEquality BIT = 1,
	@CheckEndDateEquality BIT =1
)
AS
BEGIN
	IF EXISTS	(SELECT TOP 1 1 FROM dbo.TimeEntries AS te 
				 WHERE te.MilestonePersonId = @MilestonePersonId
					   AND ((te.MilestoneDate > @StartDate OR (te.MilestoneDate = @StartDate AND @CheckStartDateEquality =1))
							AND (te.MilestoneDate < @EndDate OR (te.MilestoneDate = @EndDate AND @CheckEndDateEquality =1))
							OR @StartDate IS NULL OR @EndDate IS NULL
							)
				)
	BEGIN
		SELECT CONVERT(BIT,1) Result
	END
	ELSE
	BEGIN
		SELECT CONVERT(BIT,0) Result
	END
END
