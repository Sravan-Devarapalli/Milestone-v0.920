﻿CREATE PROCEDURE TimeEntriesGetByProject
	@ProjectId	INT,
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL,
	@PersonIds  VARCHAR(MAX) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select te.*
	from v_TimeEntries as te
	where te.ProjectId = @ProjectId
		AND te.MilestoneDate between ISNULL(@StartDate, te.MilestoneDate) and ISNULL(@EndDate, te.MilestoneDate)
		AND ((@PersonIds IS NULL) OR (te.PersonId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PersonIds))))
	order by te.PersonId, te.MilestoneDate
END

