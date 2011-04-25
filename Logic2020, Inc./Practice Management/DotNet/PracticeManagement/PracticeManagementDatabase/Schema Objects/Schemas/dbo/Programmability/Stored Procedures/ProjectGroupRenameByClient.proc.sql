CREATE PROCEDURE ProjectGroupRenameByClient
	@ClientId		INT,
	@GroupId    INT ,
	@GroupName	NVARCHAR(100),
	@IsActive   BIT 
AS
	SET NOCOUNT ON;
		UPDATE ProjectGroup
		SET Name = @GroupName , Active = @IsActive
		WHERE ClientId = @ClientId AND GroupId = @GroupId 

	SELECT 0 Result

