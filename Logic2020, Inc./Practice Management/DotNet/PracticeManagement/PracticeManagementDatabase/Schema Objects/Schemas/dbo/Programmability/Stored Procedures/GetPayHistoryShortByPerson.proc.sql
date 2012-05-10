-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 05-10-2012
-- Description:	Retrieves a payment history for the specified person.
-- =============================================
CREATE PROCEDURE [dbo].[GetPayHistoryShortByPerson]
(
	@PersonId   INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT P.StartDate,
	       P.EndDate,
	       P.Timescale
	  FROM dbo.Pay AS P
	  WHERE p.PersonId = @PersonId
	  ORDER BY p.StartDate

END

