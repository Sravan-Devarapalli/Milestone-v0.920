CREATE PROCEDURE dbo.ClientListAll
	@ShowAll BIT = 0,
	@PersonId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	--As per #2930, Not using Person Detail page Permissions.
	SELECT distinct C.ClientId
				, c.DefaultDiscount
				, c.DefaultTerms
				, c.DefaultSalespersonId
				, c.DefaultDirectorID
				, c.[Name]
				, c.Inactive
				, c.IsChargeable
	FROM Client C
	JOIN Project P ON P.ClientId = C.ClientId
	LEFT JOIN Person CD ON CD.PersonId = p.DirectorId
	LEFT JOIN Commission Cm ON Cm.ProjectId = p.ProjectId AND Cm.CommissionType = 1
	LEFT JOIN Person SP ON SP.PersonId = Cm.PersonId
	LEFT JOIN Person PM ON PM.PersonId = p.ProjectManagerId
	WHERE ((@ShowAll = 0 AND C.Inactive = 0) OR @ShowAll <> 0)
	AND	(@PersonId IS null
		OR CD.PersonId = @PersonId 
		OR SP.PersonId = @PersonId
		OR PM.PersonId = @PersonId
		)
	ORDER BY C.Name
END

