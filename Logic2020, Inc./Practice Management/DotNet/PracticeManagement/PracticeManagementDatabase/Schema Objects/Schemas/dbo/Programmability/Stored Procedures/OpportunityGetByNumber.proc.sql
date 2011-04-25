

CREATE PROCEDURE dbo.OpportunityGetByNumber
(
	@OpportunityNumber   NVARCHAR(12)
)
AS
	SET NOCOUNT ON

	SELECT o.OpportunityId
	FROM Opportunity AS o
	WHERE o.OpportunityNumber = @OpportunityNumber

