-- =============================================
-- Author:		ThulasiRam.P
-- Updated date: 06-08-2012
-- Description:	Update Status for a client.
-- =============================================
CREATE PROCEDURE dbo.UpdateIsChargableForClient
(
	@ClientID           INT,
	@IsChargeable       BIT,
	@UserLogin          NVARCHAR(255)
)

AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	Update dbo.Client
	SET IsChargeable = @IsChargeable
	WHERE ClientId = @ClientID
END
