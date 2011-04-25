CREATE PROCEDURE dbo.MembershipAliasUpdate
(
	@OldAlias	NVARCHAR(256),
	@NewAlias	NVARCHAR(256)
)
AS
BEGIN

	DECLARE @UserId  uniqueidentifier
	SELECT @UserId = UserId
	FROM aspnet_Users
	WHERE UserName = @OldAlias
	
	UPDATE 	aspnet_Users
	SET UserName = @NewAlias,
		LoweredUserName = LOWER(@NewAlias)
	WHERE UserId = @UserId
	
	UPDATE aspnet_Membership
	SET Email = @NewAlias,
		LoweredEmail = LOWER(@NewAlias)
	WHERE UserId = @UserId

END
