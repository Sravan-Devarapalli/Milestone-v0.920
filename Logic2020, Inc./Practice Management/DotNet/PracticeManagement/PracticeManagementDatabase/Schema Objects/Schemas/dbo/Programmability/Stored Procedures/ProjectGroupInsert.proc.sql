CREATE PROCEDURE ProjectGroupInsert
	@GroupId	INT OUT,
	@ClientId	INT,
	@Name		NVARCHAR(100),
	@IsActive   BIT 
AS
	SET NOCOUNT ON
	IF NOT EXISTS(SELECT 1 FROM Client WHERE ClientId=@ClientId)
		OR EXISTS(SELECT 1 FROM ProjectGroup WHERE ClientId=@ClientId AND Name=@Name)
		SET @GroupId = -1
	ELSE
	BEGIN
		INSERT ProjectGroup(ClientId, Name,Active) VALUES (@ClientId, @Name,@IsActive)
		SET @GroupId = SCOPE_IDENTITY()
	END
	 

