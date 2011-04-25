CREATE PROCEDURE [dbo].[TimeEntriesRemove]
	@MilestonePersonId int, 
	@TimeTypeId int,
	@StartDate datetime,
	@EndDate datetime
AS
	delete 
	from TimeEntries
	where 
		MilestonePersonId = @MilestonePersonId and 
		TimeTypeId = @TimeTypeId and 
		MilestoneDate between @StartDate and @EndDate
