CREATE PROCEDURE [dbo].[GetBadgeDetailsByPersonId]
(
	@PersonId		INT
)
AS
BEGIN
	SELECT	M.PersonId,
			P.FirstName,
			P.LastName,
			V.BadgeStartDate,
			V.BadgeEndDate,
			V.ProjectPlannedEndDate AS PlannedEndDate,
			V.BreakStartDate,
			V.BreakEndDate,
			M.IsBlocked,
			M.BlockStartDate,
			M.BlockEndDate,
			M.IsPreviousBadge,
			M.PreviousBadgeAlias,
			M.LastBadgeStartDate,
			M.LastBadgeEndDate,
			M.IsException,
			M.ExceptionStartDate,
			M.ExceptionEndDate,
			V.BadgeStartDateSource,
			V.PlannedEndDateSource,
			V.BadgeEndDateSource,
			M.DeactivatedDate,
			M.OrganicBreakStartDate,
			M.OrganicBreakEndDate,
			M.ExcludeInReports,
			M.ManageServiceContract
	FROM dbo.MSBadge M
	INNER JOIN dbo.Person P ON P.PersonId = M.PersonId
	LEFT JOIN v_CurrentMSBadge V ON V.PersonId = M.PersonId
	WHERE M.PersonId = @PersonId
END

