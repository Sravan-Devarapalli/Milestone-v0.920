﻿CREATE PROCEDURE [dbo].[GetOpportunityPersons]
	@OpportunityId int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM dbo.OpportunityPersons WHERE OpportunityId = @OpportunityId)
	BEGIN
		SELECT   op.PersonId
				,p.FirstName
				,p.LastName,
				op.OpportunityPersonTypeId,
				op.RelationTypeId,
				op.Quantity,
				op.NeedBy
		FROM dbo.OpportunityPersons op
		JOIN dbo.Person p ON p.PersonId = op.PersonId
		WHERE op.OpportunityId = @OpportunityId
				AND p.PersonStatusId IN(1,3)
				
	END
END
