CREATE PROCEDURE [dbo].[IsOpportunityHaveTeamStructure]
@OpportunityId INT	
AS
BEGIN
	SELECT COUNT(*)
	FROM  OpportunityPersons AS o 
	WHERE o.OpportunityId = @OpportunityId
END

