CREATE PROCEDURE [dbo].[CSATDelete]
(
	@ProjectCSATId		INT, 
	@UserLogin      NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DELETE dbo.ProjectCSAT 
	WHERE [CSATId] = @ProjectCSATId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
