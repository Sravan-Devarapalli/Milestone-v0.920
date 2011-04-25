
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-12-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 11-07-2008
-- Description:	Updates a PersonCalendar table according to the data in the Person table
-- =============================================
CREATE TRIGGER [dbo].[tr_Person_UpdateCalendar]
ON [dbo].[Person]
AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON

	-- Deleting redundand records
	DELETE pc
	  FROM dbo.PersonCalendarAuto AS pc
	       INNER JOIN inserted AS p ON pc.PersonId = p.PersonId
	 WHERE NOT EXISTS (SELECT 1
	                     FROM deleted AS d
	                    WHERE d.PersonId = p.PersonId
	                      AND d.HireDate = p.HireDate
	                      AND ISNULL(d.TerminationDate, '1900-01-01') = ISNULL(p.TerminationDate, '1900-01-01'))

	-- Inserting new records
	INSERT INTO dbo.PersonCalendarAuto
	            (Date, PersonId, DayOff)
	     SELECT c.Date, c.PersonId, c.DayOff
	       FROM dbo.v_PersonCalendar AS c
	            INNER JOIN inserted AS p ON c.PersonId = p.PersonId
	      WHERE NOT EXISTS (SELECT 1
	                          FROM deleted AS d
	                         WHERE d.PersonId = p.PersonId
	                           AND d.HireDate = p.HireDate
	                           AND ISNULL(d.TerminationDate, '1900-01-01') = ISNULL(p.TerminationDate, '1900-01-01'))

END

