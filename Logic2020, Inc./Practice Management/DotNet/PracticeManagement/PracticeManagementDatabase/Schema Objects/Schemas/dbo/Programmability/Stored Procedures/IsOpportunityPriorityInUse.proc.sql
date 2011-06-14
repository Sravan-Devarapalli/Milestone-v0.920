CREATE PROCEDURE [dbo].[IsOpportunityPriorityInUse]
@PriorityId INT	
AS
BEGIN
	SELECT COUNT(*)
	FROM  v_Opportunity AS o 
	WHERE o.PriorityId = @PriorityId
END

