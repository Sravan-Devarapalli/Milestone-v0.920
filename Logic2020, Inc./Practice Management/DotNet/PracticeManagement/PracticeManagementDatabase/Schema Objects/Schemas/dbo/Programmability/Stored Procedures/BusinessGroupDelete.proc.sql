CREATE PROCEDURE [dbo].BusinessGroupDelete
(
	@BusinessGroupId		INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	
	DELETE dbo.BusinessGroup 
	WHERE BusinessGroupId = @BusinessGroupId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
