CREATE PROCEDURE [dbo].[OpportunityPersonInsert]
(
	@OpportunityId INT, 
	@PersonIdList NVARCHAR(MAX),
	@OutSideResources NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @OpportunityIdsLocal NVARCHAR(MAX)
	SELECT @OpportunityIdsLocal = @PersonIdList
	DECLARE @OpportunityIdTable TABLE
	(
		OpportunityId INT
	)
	
	INSERT INTO @OpportunityIdTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@OpportunityIdsLocal)
	
	IF(@PersonIdList IS NOT NULL)
	BEGIN
		DELETE op
		FROM dbo.OpportunityPersons  op
		LEFT JOIN @OpportunityIdTable AS p 
		ON op.OpportunityId = @OpportunityId AND op.PersonId = p.OpportunityId
		WHERE p.OpportunityId IS NULL and OP.OpportunityId = @OpportunityId

		INSERT INTO OpportunityPersons
		SELECT @OpportunityId ,P.OpportunityId
		FROM @OpportunityIdTable AS p 
		LEFT JOIN dbo.OpportunityPersons op
		ON p.OpportunityId = op.PersonId AND op.OpportunityId=@OpportunityId
		WHERE op.PersonId IS NULL 
	END
	UPDATE dbo.Opportunity
	SET OutSideResources = @OutSideResources
	WHERE OpportunityId = @OpportunityId
END
