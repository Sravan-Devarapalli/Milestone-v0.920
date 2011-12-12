-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-02-02
-- Description:	Removes TE
-- =============================================
CREATE PROCEDURE dbo.TimeEntryRemove
	@TimeEntryId INT 
AS
BEGIN
	SET NOCOUNT ON;
	

	DECLARE @PTOTimeTypeId INT,
		@TimeTypeId INT,
		@PersonID INT,
		@MilestonePersonId INT,
		@MilestoneDate DATETIME

	SELECT @PTOTimeTypeId = TimeTypeId
	FROM TimeType
	WHERE Name = 'PTO'

	SELECT @TimeTypeId = TimeTypeId, @PersonID = MP.PersonId, @MilestonePersonId = TE.MilestonePersonId, @MilestoneDate = TE.MilestoneDate
	FROM TimeEntries TE
	JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId
	WHERE TimeEntryId = @TimeEntryId
	
	DELETE 
	FROM TimeEntries 
	WHERE TimeEntryId = @TimeEntryId	

	IF @TimeTypeId = @PTOTimeTypeId
		AND NOT EXISTS (SELECT 1 FROM TimeEntries TE
								JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId AND MP.PersonId = @PersonId
								WHERE TE.MilestoneDate = @MilestoneDate 
								AND TE.TimeTypeId = @PTOTimeTypeId
						)
	BEGIN
		DELETE 
		FROM PersonCalendar
		WHERE PersonId = @PersonID AND Date = @MilestoneDate

		UPDATE ca
			SET DayOff = pc.DayOff
			FROM dbo.PersonCalendarAuto AS ca
				INNER JOIN dbo.v_PersonCalendar AS pc ON ca.date = pc.Date AND ca.PersonId = pc.PersonId AND pc.Date = @MilestoneDate
			WHERE ca.PersonId = @PersonId
	END

END

