CREATE PROCEDURE dbo.UpdateMinimumLoadFactorStatus
(
	@InActive	BIT
)
AS
BEGIN
	UPDATE dbo.OverheadFixedRate
	SET Inactive = @InActive
	WHERE IsMinimumLoadFactor = 1
END
