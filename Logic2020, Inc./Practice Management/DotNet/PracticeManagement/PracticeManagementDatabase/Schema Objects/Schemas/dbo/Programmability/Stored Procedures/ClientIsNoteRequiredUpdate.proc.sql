-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 06-08-2012
-- Description:	Update IsNoteRequired for a client.
-- =============================================
CREATE PROCEDURE [dbo].[ClientIsNoteRequiredUpdate]
(
	@ClientID INT,
	@IsNoteRequired     BIT = 1,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	Update dbo.Client
	SET IsNoteRequired = @IsNoteRequired
	WHERE ClientId = @ClientID

END

