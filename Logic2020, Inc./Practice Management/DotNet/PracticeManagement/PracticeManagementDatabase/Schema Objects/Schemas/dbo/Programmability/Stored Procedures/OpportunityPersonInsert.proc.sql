CREATE PROCEDURE [dbo].[OpportunityPersonInsert]
(
	@OpportunityId INT, 
	@PersonIdList NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.OpportunityPersons(OpportunityId, PersonId)
	SELECT @OpportunityId AS OpportunityId, ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PersonIdList)
END
