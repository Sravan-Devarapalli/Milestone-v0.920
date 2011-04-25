CREATE PROCEDURE dbo.OpportunityPrioritiesListAll
AS
BEGIN
	SET NOCOUNT ON
	SELECT	Priority,
			Description
	FROM dbo.OpportunityPriorities
	ORDER BY Priority
END
