CREATE PROCEDURE [dbo].[GetStrawManListAllShort]
(
	@IncludeInactive BIT = 1
)
AS
BEGIN
	SELECT	P.PersonId,
			P.LastName,
			P.FirstName
	FROM dbo.Person P
	WHERE P.IsStrawman = 1 
	AND (
		@IncludeInactive = 1 
		OR P.PersonStatusId = 1  --active status
		)
	ORDER BY P.LastName,P.FirstName
END
