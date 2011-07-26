CREATE PROCEDURE [dbo].[GetActiveOpportunitiesByOwnerId]
(
	@PersonId	INT
)
AS
BEGIN 

	SELECT op.OpportunityId,
		   op.Name OpportunityName
	FROM dbo.Opportunity AS op 
	WHERE op.OwnerId =@PersonId AND op.OpportunityStatusId =1
	
END
