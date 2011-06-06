CREATE PROCEDURE dbo.CheckPersonTimeEntriesAfterTerminationDate
(
	@PersonId	INT,
	@TerminationDate	DATETIME
)
AS
BEGIN
	DECLARE @TimeEntriesExists BIT
	IF EXISTS(
					SELECT 1 FROM dbo.TimeEntries TE
					JOIN dbo.MilestonePerson MP ON TE.MilestonePersonId = MP.MilestonePersonId
					WHERE MP.PersonId = @PersonId AND TE.MilestoneDate >= @TerminationDate
				)

		SET @TimeEntriesExists  = 1
	ELSE
		SET @TimeEntriesExists = 0

	SELECT @TimeEntriesExists

END
