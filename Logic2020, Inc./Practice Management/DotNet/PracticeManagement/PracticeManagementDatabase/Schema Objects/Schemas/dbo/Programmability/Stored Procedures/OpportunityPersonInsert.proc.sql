CREATE PROCEDURE [dbo].[OpportunityPersonInsert]
(
	@OpportunityId INT, 
	@PersonIdList NVARCHAR(MAX),
	@OutSideResources NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PersonIdListLocal NVARCHAR(MAX)
	SELECT  @PersonIdListLocal = @PersonIdList
	DECLARE @OpportunityPersonIdsWithTypeTable TABLE
	(
		PersonId INT,
		PersonType INT
	)
	
	INSERT INTO @OpportunityPersonIdsWithTypeTable
	SELECT ResultId ,ResultType
	FROM [dbo].[ConvertStringListIntoTableWithTwoColoumns] (@PersonIdListLocal)
	
	IF(@PersonIdList IS NOT NULL)
	BEGIN
		DELETE op
		FROM dbo.OpportunityPersons  op
		LEFT JOIN @OpportunityPersonIdsWithTypeTable AS p 
		ON op.OpportunityId = @OpportunityId AND op.PersonId = p.PersonId AND op.OpportunityPersonTypeId=p.PersonType
		WHERE p.PersonId IS NULL and OP.OpportunityId = @OpportunityId

		INSERT INTO OpportunityPersons(OpportunityId,PersonId,OpportunityPersonTypeId)
		SELECT @OpportunityId ,p.PersonId,p.PersonType
		FROM @OpportunityPersonIdsWithTypeTable AS p 
		LEFT JOIN dbo.OpportunityPersons op
		ON p.PersonId = op.PersonId AND op.OpportunityId=@OpportunityId AND op.OpportunityPersonTypeId=p.PersonType
		WHERE op.PersonId IS NULL 
	END
	UPDATE dbo.Opportunity
	SET OutSideResources = @OutSideResources
	WHERE OpportunityId = @OpportunityId
END
