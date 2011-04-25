-- =============================================
-- Author:		Skip Sailors
-- Create date: 4-22-2008
-- Description:	Mark a client active
-- =============================================
CREATE PROCEDURE [dbo].[ClientReactivate] 
	@ClientID int
AS
BEGIN
	SET NOCOUNT ON;

	Update Client
		Set Inactive = 0
		Where ClientId = @ClientID
END

