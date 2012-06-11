-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	ThulasiRam.P
-- Update date: 6-08-2012
-- Description:	Gets a client
-- =============================================
CREATE PROCEDURE dbo.ClientGetById
(
	@ClientId	INT
)
AS
	SET NOCOUNT ON;

	SELECT c.ClientId,
	       c.DefaultDiscount,
	       c.DefaultTerms,
	       c.DefaultSalespersonID,
		   c.DefaultDirectorID,
	       c.Name,
	       c.Inactive,
	       c.IsChargeable,
		   c.IsMarginColorInfoEnabled,
		   c.IsInternal,
		   c.Code AS ClientCode,
		   c.IsNoteRequired
	  FROM dbo.Client AS c
	 WHERE c.ClientId = @ClientId

GO

