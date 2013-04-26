CREATE PROCEDURE [dbo].[GetPricingList]
(
	@ClientId	INT = NULL
)
AS
BEGIN
	SELECT PricingListId,
			ClientId,
			Name,
			dbo.IsPricingListInUse(pl.PricingListId,pl.IsDefault) AS InUse,
			IsDefault,
			IsActive
	FROM dbo.PricingList pl
	WHERE  @ClientId IS NULL OR pl.ClientId = @ClientId
END
