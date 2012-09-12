CREATE PROCEDURE [dbo].[GetActiveOpportunitiesByOwnerId]
(
	@PersonId	INT
)
AS
BEGIN 

	SELECT op.OpportunityId,
		   op.Name OpportunityName,
		   op.OpportunityNumber
	FROM dbo.Opportunity AS op 
	WHERE op.OwnerId = @PersonId AND op.OpportunityStatusId =1
	ORDER BY op.OpportunityNumber
	
END
