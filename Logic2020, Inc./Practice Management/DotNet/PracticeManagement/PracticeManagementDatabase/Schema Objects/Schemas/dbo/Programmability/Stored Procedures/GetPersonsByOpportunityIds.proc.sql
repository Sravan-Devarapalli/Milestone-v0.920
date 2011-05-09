CREATE PROCEDURE dbo.GetPersonsByOpportunityIds
(
	@OpportunityIds NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @OpportunityIdsLocal NVARCHAR(MAX)
	SELECT @OpportunityIdsLocal = @OpportunityIds
	
	DECLARE @OpportunityIdTable TABLE
	(
		OpportunityId INT
	)
	
	INSERT INTO @OpportunityIdTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@OpportunityIdsLocal)
	
	SELECT   op.PersonId
				,p.FirstName
				,p.LastName,
				op.OpportunityId
		FROM dbo.OpportunityPersons op
		JOIN dbo.Person p ON p.PersonId = op.PersonId
		WHERE op.OpportunityId IN (SELECT OpportunityId FROM @OpportunityIdTable)
				AND p.PersonStatusId IN(1,3)
		ORDER BY op.OpportunityId
END
