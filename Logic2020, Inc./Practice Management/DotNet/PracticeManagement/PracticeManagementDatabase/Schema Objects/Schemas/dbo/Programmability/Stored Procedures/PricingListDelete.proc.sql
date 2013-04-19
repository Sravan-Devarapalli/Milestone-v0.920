CREATE PROCEDURE [dbo].[PricingListDelete]
(
	@PricingListId INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DELETE dbo.PricingList 
	WHERE PricingListId = @PricingListId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
