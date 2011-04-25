-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-12-2008
-- Description:	Updates a calendar item
-- =============================================
CREATE PROCEDURE [dbo].[CalendarUpdate]
(
	@Date       DATETIME,
	@DayOff     BIT,
	@PersonId   INT
)
AS
	SET NOCOUNT ON
	
	IF @PersonId IS NULL
	BEGIN
		-- Update the company calendar
		UPDATE dbo.Calendar
		   SET DayOff = @DayOff
		 WHERE Date = @Date
	END
	ELSE IF EXISTS (SELECT 1
	                  FROM dbo.Calendar AS cal
	                 WHERE cal.Date = @Date AND cal.DayOff = @DayOff)
	BEGIN
		-- Clear the person record
		DELETE
		  FROM dbo.PersonCalendar
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE IF EXISTS (SELECT 1
	                  FROM dbo.PersonCalendar AS pcal
	                 WHERE pcal.Date = @Date AND pcal.PersonId = @PersonId)
	BEGIN
		-- Update an existing person record
		UPDATE dbo.PersonCalendar
		   SET DayOff = @DayOff
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE
	BEGIN
		-- A person record does not exist - create it
		INSERT INTO dbo.PersonCalendar
		            (Date, PersonId, DayOff)
		     VALUES (@Date, @PersonId, @DayOff)
	END

	UPDATE ca
	   SET DayOff = pc.DayOff
	  FROM dbo.PersonCalendarAuto AS ca
	       INNER JOIN dbo.v_PersonCalendar AS pc ON ca.date = pc.Date AND ca.PersonId = pc.PersonId
	 WHERE ca.Date = @Date
	   AND (@PersonId IS NULL OR ca.PersonId = @PersonId)

