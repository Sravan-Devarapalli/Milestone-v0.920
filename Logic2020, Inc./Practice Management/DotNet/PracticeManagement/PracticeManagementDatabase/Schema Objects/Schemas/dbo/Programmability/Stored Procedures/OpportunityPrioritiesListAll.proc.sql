CREATE PROCEDURE dbo.OpportunityPrioritiesListAll
AS
BEGIN
	SET NOCOUNT ON
	SELECT	Id,
			Priority,
			Description
	FROM dbo.OpportunityPriorities AS OP
	ORDER BY OP.sortOrder
END
