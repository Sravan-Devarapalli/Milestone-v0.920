CREATE PROCEDURE [dbo].[BusinessGroupUpdate]
(
	  @BusinessGroupId	INT,
	  @Name		NVARCHAR(100),
	  @IsActive   BIT ,
	  @UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	UPDATE BusinessGroup
	SET Name = @Name,
		Active = @IsActive
	WHERE BusinessGroupId = @BusinessGroupId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
