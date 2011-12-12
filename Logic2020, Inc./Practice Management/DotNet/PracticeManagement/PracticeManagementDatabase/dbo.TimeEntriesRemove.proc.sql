CREATE PROCEDURE [dbo].[TimeEntriesRemove]
	@MilestonePersonId int, 
	@TimeTypeId int,
	@StartDate datetime,
	@EndDate datetime
AS
BEGIN

	DECLARE @PTOTimeTypeId INT,
		@PersonId INT
		
	SELECT @PTOTimeTypeId = TimeTypeId
	FROM TimeType
	WHERE Name = 'PTO'

	SELECT @PersonId = PersonId
	FROM MilestonePerson
	WHERE MilestonePersonId = @MilestonePersonId
	
	DELETE PC
	FROM TimeEntries TE
	JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId
	JOIN PersonCalendar PC ON PC.PersonId = MP.PersonId AND TE.MilestoneDate = PC.Date
	WHERE TE.MilestonePersonId = @MilestonePersonId 
	AND TE.MilestoneDate BETWEEN @StartDate AND @EndDate
	AND TE.TimeTypeId = @PTOTimeTypeId 
	AND NOT EXISTS (SELECT 1 FROM TimeEntries TE2
					JOIN MilestonePerson MP2 ON MP2.MilestonePersonId = TE2.MilestonePersonId AND MP2.PersonId = MP.PersonId
					WHERE TE2.MilestoneDate = TE.MilestoneDate AND MP2.MilestonePersonId <> MP.MilestonePersonId
					AND TE2.TimeTypeId = @PTOTimeTypeId)
	UPDATE ca
	SET DayOff = pc.DayOff
	FROM dbo.PersonCalendarAuto AS ca
	INNER JOIN dbo.v_PersonCalendar AS pc ON ca.date = pc.Date AND ca.PersonId = pc.PersonId AND pc.Date BETWEEN @StartDate AND @EndDate
	WHERE ca.PersonId = @PersonId

	
	delete 
	from TimeEntries
	where 
		MilestonePersonId = @MilestonePersonId and 
		TimeTypeId = @TimeTypeId and 
		MilestoneDate between @StartDate and @EndDate

END
