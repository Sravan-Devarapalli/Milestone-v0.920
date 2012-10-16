CREATE PROCEDURE dbo.OpportunityPrioritiesListAll
AS
BEGIN
	SET NOCOUNT ON
	SELECT	Id,
			Priority,
			Description,
			DisplayName
	FROM dbo.OpportunityPriorities AS OP
	ORDER BY OP.sortOrder
END
