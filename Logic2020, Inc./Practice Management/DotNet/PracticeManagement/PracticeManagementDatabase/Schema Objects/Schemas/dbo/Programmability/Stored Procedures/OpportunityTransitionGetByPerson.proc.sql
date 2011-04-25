CREATE PROCEDURE OpportunityTransitionGetByPerson 
	@PersonId INT 
AS
BEGIN
	SET NOCOUNT ON;

	select 
		ot.OpportunityTransitionId,
		ot.OpportunityTransitionStatusId,
		ot.OpportunityTransitionStatusName,
		ot.OpportunityId,
		ot.TargetPersonId,
		ot.OpportunityNumber,
		ot.OpportunityName,
		ot.Priority,
		ot.ClientName,
		ot.LastUpdated as 'LastUpdate'
	from v_OpportunityTransition as ot
	where ot.TargetPersonId = @PersonId
			AND ot.OpportunityStatusId = 1 --get only active Opportunities according to bug# 2608
END

