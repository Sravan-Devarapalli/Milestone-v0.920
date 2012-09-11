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
				op.OpportunityId,
				op.OpportunityPersonTypeId,
				op.RelationTypeId,
				op.Quantity,
				op.NeedBy,
				p.PersonStatusId 
		FROM dbo.OpportunityPersons op
		INNER JOIN dbo.Person p ON p.PersonId = op.PersonId
		INNER JOIN @OpportunityIdTable O ON O.OpportunityId = op.OpportunityId
		WHERE (
				p.PersonStatusId IN(1,3,5)
				OR
				p.IsStrawman = 1
			)
		ORDER BY op.OpportunityId
END

