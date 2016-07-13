CREATE PROCEDURE [dbo].[GetPersonDivisions]
AS
BEGIN
	SELECT pd.DivisionId,
		   pd.DivisionName,
		   pd.Inactive,
		   pers.FirstName,
		   pers.LastName,
		   pers.PersonId
		   FROM dbo.persondivision pd
		   LEFT JOIN dbo.Person AS pers ON pd.DivisionOwnerId = pers.PersonId
		   WHERE Inactive=0
		   ORDER BY pd.DivisionName
END 

