CREATE PROCEDURE [dbo].[GetOpportunityPersons]
	@OpportunityId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT   op.PersonId
			,p.FirstName
			,p.LastName,
			op.OpportunityPersonTypeId,
			op.RelationTypeId,
			op.Quantity,
			op.NeedBy,
			p.PersonStatusId 
	FROM dbo.OpportunityPersons op
	INNER JOIN dbo.Person p ON p.PersonId = op.PersonId
	WHERE op.OpportunityId = @OpportunityId
			AND (
					p.PersonStatusId IN(1,3,5)
					OR
					p.IsStrawman = 1
				)
END
