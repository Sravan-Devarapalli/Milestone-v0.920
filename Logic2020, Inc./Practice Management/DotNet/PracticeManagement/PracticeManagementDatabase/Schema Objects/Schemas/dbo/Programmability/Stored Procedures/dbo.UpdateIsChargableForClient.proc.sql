CREATE PROCEDURE dbo.UpdateIsChargableForClient 
	@ClientID             INT,
	@IsChargeable         BIT
AS
BEGIN
	SET NOCOUNT ON;

	Update Client
		Set IsChargeable = @IsChargeable
		Where ClientId = @ClientID
END
