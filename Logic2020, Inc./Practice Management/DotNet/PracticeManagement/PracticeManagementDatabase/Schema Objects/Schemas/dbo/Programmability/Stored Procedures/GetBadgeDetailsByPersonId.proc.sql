CREATE PROCEDURE [dbo].[GetBadgeDetailsByPersonId]
(
	@PersonId		INT
)
AS
BEGIN
	SELECT	M.PersonId,
			P.FirstName,
			P.LastName,
			BadgeStartDate,
			BadgeEndDate,
			PlannedEndDate,
			BreakStartDate,
			BreakEndDate,
			IsBlocked,
			BlockStartDate,
			BlockEndDate,
			IsPreviousBadge,
			PreviousBadgeAlias,
			LastBadgeStartDate,
			LastBadgeEndDate,
			IsException,
			ExceptionStartDate,
			ExceptionEndDate,
			BadgeStartDateSource,
			PlannedEndDateSource,
			BadgeEndDateSource
	FROM dbo.MSBadge M
	INNER JOIN dbo.Person P ON P.PersonId = M.PersonId
	WHERE M.PersonId = @PersonId
END

