-- =============================================
-- Author:		Skip Sailors
-- Create date: 4-22-1008
-- Description:	Mark a client inactive
-- =============================================
CREATE PROCEDURE [dbo].[ClientInactivate] 
	-- Add the parameters for the stored procedure here
	@ClientID int
AS
BEGIN
	SET NOCOUNT ON;

	Update Client
		Set Inactive = 1
		Where ClientId = @ClientID
END

