CREATE PROCEDURE [dbo].[GetListOfOfferings]
AS
BEGIN
	SELECT O.OfferingId,
		   O.Name
	FROM Offering O
	ORDER BY O.Name
END
