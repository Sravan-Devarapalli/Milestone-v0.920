CREATE PROCEDURE [dbo].PricingListUpdate
(
	  @PricingListId	INT,
	  @Name	    	    NVARCHAR(200),
	  @UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	UPDATE PricingList
	SET Name = @Name
	WHERE PricingListId = @PricingListId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
