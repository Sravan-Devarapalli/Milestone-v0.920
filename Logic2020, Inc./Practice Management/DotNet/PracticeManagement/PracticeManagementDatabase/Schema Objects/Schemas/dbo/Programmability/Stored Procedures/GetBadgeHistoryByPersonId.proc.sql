CREATE PROCEDURE [dbo].[GetBadgeHistoryByPersonId]
(
	@PersonId	INT
)
AS
BEGIN
	--Block History
	SELECT B.PersonId,
		   P.FirstName,
		   P.LastName,
		   B.BlockStartDate,
		   B.BlockEndDate,
		   B.ModifiedDate,
		   B.ModifiedBy AS ModifiedById,
		   ISNULL(Updated.LastName+', '+Updated.FirstName,'pracadmin') AS ModifiedBy
	FROM dbo.BlockHistory B
	JOIN dbo.Person P ON P.PersonId = B.PersonId
	LEFT JOIN dbo.Person Updated ON Updated.PersonId = B.ModifiedBy
	WHERE B.PersonId = @PersonId
	ORDER BY B.ModifiedDate DESC

	--Override Exception History
	SELECT O.PersonId,
		   P.FirstName,
		   P.LastName,
		   O.OverrideStartDate,
		   O.OverrideEndDate,
		   O.ModifiedDate,
		   O.ModifiedBy AS ModifiedById,
		   ISNULL(Updated.LastName+', '+Updated.FirstName,'pracadmin') AS ModifiedBy
	FROM dbo.OverrideExceptionHistory O
	JOIN dbo.Person P ON P.PersonId = O.PersonId
	LEFT JOIN dbo.Person Updated ON Updated.PersonId = O.ModifiedBy
	WHERE O.PersonId = @PersonId
	ORDER BY O.ModifiedDate DESC

	--18 mo details History
	SELECT B.PersonId,
		   P.FirstName,
		   P.LastName,
		   B.BadgeStartDate,
		   b.ProjectPlannedEndDate,
		   B.BadgeEndDate,
		   b.BreakStartDate,
		   b.BreakEndDate,
		   b.BadgeStartDateSource,
		   b.ProjectPlannedEndDateSource,
		   b.BadgeEndDateSource,
		   B.ModifiedDate,
		   B.ModifiedBy AS ModifiedById,
		   ISNULL(Updated.LastName+', '+Updated.FirstName,'pracadmin') AS ModifiedBy
	FROM dbo.BadgeHistory B
	JOIN dbo.Person P ON P.PersonId = B.PersonId
	LEFT JOIN dbo.Person Updated ON Updated.PersonId = B.ModifiedBy
	WHERE B.PersonId = @PersonId
	ORDER BY B.ModifiedDate DESC

END
