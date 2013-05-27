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
		   c.IsNoteRequired,
		   c.IsHouseAccount
	  FROM dbo.Client AS c
	 WHERE c.ClientId = @ClientId

GO

