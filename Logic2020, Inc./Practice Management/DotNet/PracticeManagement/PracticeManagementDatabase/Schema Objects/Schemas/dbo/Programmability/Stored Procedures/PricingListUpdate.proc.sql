CREATE PROCEDURE [dbo].PricingListUpdate
(
	  @PricingListId	INT,
	  @Name	    	    NVARCHAR(200),
  	  @IsActive	    	    BIT,
	  @UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	UPDATE PricingList
	SET Name = @Name,
	IsActive = @IsActive
	WHERE PricingListId = @PricingListId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
