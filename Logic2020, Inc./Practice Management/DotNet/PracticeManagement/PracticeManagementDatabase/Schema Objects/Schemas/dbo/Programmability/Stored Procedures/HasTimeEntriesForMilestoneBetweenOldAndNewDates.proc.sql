CREATE PROCEDURE [dbo].[HasTimeEntriesForMilestoneBetweenOldAndNewDates]
(
@MilestoneId	INT,
@StartDate   DATETIME = NULL,
@EndDate     DATETIME  = NULL
)
AS
BEGIN
DECLARE @Result BIT , 
		@MileStoneStartDate  DATETIME,
		@MileStoneEndDate  DATETIME,
		@SwapStartDate  DATETIME,
		@SwapEndDate  DATETIME

SELECT @MileStoneStartDate = m.StartDate
	   ,@MileStoneEndDate =m.ProjectedDeliveryDate
	   FROM Milestone AS m
	   WHERE m.MilestoneId = @MilestoneId

IF(@StartDate > @MileStoneStartDate)
BEGIN
  SET @SwapStartDate = @StartDate
  SELECT @StartDate = @MileStoneStartDate , @MileStoneStartDate = @SwapStartDate
END

IF(@EndDate > @MileStoneEndDate)
BEGIN
  SET @SwapEndDate = @EndDate
  SELECT @EndDate = @MileStoneEndDate , @MileStoneEndDate = @SwapEndDate
END

IF EXISTS(SELECT 1 FROM v_TimeEntries as te	where te.MilestoneId = @MilestoneId AND
			 MilestoneDate  BETWEEN @StartDate AND @MileStoneStartDate)
BEGIN 
	SELECT @Result=1
END
ELSE IF EXISTS(SELECT 1 FROM v_TimeEntries as te	where te.MilestoneId = @MilestoneId AND
			 MilestoneDate  BETWEEN @EndDate AND @MileStoneEndDate)
BEGIN 
	SELECT @Result=1
END
ELSE
BEGIN
	SELECT @Result=0
END
	SELECT @Result AS HasTimeEntries
END
GO
