CREATE FUNCTION [dbo].[IsPricingListInUse]
(
	@PricingListId INT,
	@IsDefault BIT
)
RETURNS BIT
AS
BEGIN
	IF(@IsDefault = 1)
	BEGIN
		RETURN 1
	END
	IF EXISTS(SELECT 1 FROM dbo.Project WHERE PricingListId = @PricingListId)
	BEGIN
		RETURN 1
	END
	IF EXISTS(SELECT 1 FROM dbo.Opportunity WHERE PricingListId = @PricingListId)
	BEGIN
		RETURN 1
	END
		RETURN 0
END
