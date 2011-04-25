CREATE TRIGGER UpdateProjectWithLinkedOpportunity 
   ON  dbo.Opportunity 
   AFTER INSERT, UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	Declare @OpportunityId int
	Declare @ProjectId int

	select 
		@ProjectId = i.ProjectId, 
		@OpportunityId = i.OpportunityId 
	from inserted as i

	update dbo.Project
	set OpportunityId = @OpportunityId
	where ProjectId = @ProjectId
END

