--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Description:	Gets a person
-- =============================================
CREATE PROCEDURE dbo.ClientGetById
(
	@ClientId	INT
)
AS
	SET NOCOUNT ON

	SELECT c.ClientId,
	       c.DefaultDiscount,
	       c.DefaultTerms,
	       c.DefaultSalespersonID,
		   c.DefaultDirectorID,
	       c.Name,
	       c.Inactive,
	       c.IsChargeable
	  FROM dbo.Client AS c
	 WHERE c.ClientId = @ClientId

