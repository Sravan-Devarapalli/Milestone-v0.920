CREATE PROCEDURE [dbo].[DeleteQuickLinkById]
	@Id INT
AS
BEGIN

	DELETE QuickLinks
	WHERE Id =@Id

END
