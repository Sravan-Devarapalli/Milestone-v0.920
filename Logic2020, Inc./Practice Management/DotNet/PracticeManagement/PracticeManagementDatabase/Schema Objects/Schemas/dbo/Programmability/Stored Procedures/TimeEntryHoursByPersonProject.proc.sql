CREATE PROCEDURE TimeEntryHoursByPersonProject
	@PersonId	INT,
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL
AS
BEGIN
	SET NOCOUNT ON;

 	select te.MilestoneDate, te.ProjectId as [Id], te.ProjectName as [Name], SUM(te.ActualHours) as ActualHours
	from v_TimeEntries as te
	where te.MilestoneDate between @StartDate and @EndDate
			and te.PersonId = @PersonId
	group by te.MilestoneDate, te.ProjectId, te.ProjectName
	order by te.ProjectId
END

