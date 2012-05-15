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
	       CASE WHEN P.EndDate IS NULL 
		   THEN NULL ELSE  P.EndDate- 1 END AS [EndDate],
	       P.Timescale
	  FROM dbo.Pay AS P
	  WHERE p.Person = @PersonId
	  ORDER BY p.StartDate

END

