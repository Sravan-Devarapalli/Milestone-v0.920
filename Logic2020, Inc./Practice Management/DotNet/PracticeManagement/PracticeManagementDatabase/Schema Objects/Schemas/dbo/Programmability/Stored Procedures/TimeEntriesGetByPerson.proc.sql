CREATE PROCEDURE TimeEntriesGetByPerson
	@PersonId	INT,
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select te.*
	from v_TimeEntries as te
	where te.PersonId = @PersonId
		AND te.MilestoneDate between ISNULL(@StartDate, te.MilestoneDate) and ISNULL(@EndDate, te.MilestoneDate)
	order by te.ProjectId, te.MilestoneDate
END

