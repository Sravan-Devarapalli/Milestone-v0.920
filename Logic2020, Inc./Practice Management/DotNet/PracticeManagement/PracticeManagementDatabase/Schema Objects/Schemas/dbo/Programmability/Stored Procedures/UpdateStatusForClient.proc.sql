-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 06-08-2012
-- Description:	Update Status for a client.
-- =============================================
CREATE PROCEDURE dbo.UpdateStatusForClient
(
	@ClientID           INT,
	@Inactive           BIT,
	@UserLogin          NVARCHAR(255)
)

AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	Update dbo.Client
	SET Inactive = @Inactive
	WHERE ClientId = @ClientID
END
