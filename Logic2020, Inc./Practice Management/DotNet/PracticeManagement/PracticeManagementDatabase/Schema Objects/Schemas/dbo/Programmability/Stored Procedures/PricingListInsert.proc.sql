CREATE PROCEDURE [dbo].[PricingListInsert]
(
    @PricingListId	INT OUT,
	@ClientId    	INT,
	@Name		    NVARCHAR(200),
	@IsActive	    	    BIT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	INSERT PricingList(ClientId, Name,IsActive) 
	VALUES (@ClientId, @Name,@IsActive)

	SET @PricingListId = SCOPE_IDENTITY()

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
