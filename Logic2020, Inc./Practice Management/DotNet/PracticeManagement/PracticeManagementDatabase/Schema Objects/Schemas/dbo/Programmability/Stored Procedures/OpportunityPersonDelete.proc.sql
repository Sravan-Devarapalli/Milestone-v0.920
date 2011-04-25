CREATE PROCEDURE [dbo].[OpportunityPersonDelete]
(
	@OpportunityId INT,
	@PersonIdList  NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON;

	DELETE dbo.OpportunityPersons
	WHERE OpportunityId = @OpportunityId 
			AND PersonId IN(
							SELECT ResultId 
							FROM [dbo].[ConvertStringListIntoTable] (@PersonIdList)
							)
END
