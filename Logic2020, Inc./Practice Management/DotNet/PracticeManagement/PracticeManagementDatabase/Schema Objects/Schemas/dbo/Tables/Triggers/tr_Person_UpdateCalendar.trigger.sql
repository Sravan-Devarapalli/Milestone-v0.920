
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-12-2008
-- Updated by:	Srinivas Middela
-- Update date: 17-09-2012
-- Description:	Updates a PersonCalendar table according to the data in the Person table
-- =============================================
CREATE TRIGGER [dbo].[tr_Person_UpdateCalendar]
ON [dbo].[Person]
AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON

	IF EXISTS (SELECT 1
				FROM inserted i
				LEFT JOIN deleted d ON d.PersonId = i.PersonId
				WHERE ISNULL(i.HireDate, '') <> ISNULL(d.HireDate, '') OR ISNULL(i.TerminationDate, '') <> ISNULL(d.TerminationDate, ''))
	BEGIN

		-- Deleting redundand records
		DELETE PCA
		FROM deleted AS d
		INNER JOIN  dbo.PersonCalendarAuto AS PCA ON d.PersonId = PCA.PersonId
		LEFT JOIN dbo.v_PersonCalendar AS C ON C.PersonId = PCA.PersonId AND C.Date = PCA.Date
		WHERE C.Date IS NULL AND C.PersonId IS NULL

		-- Inserting new records
		INSERT INTO dbo.PersonCalendarAuto
					(Date, PersonId, DayOff)
		SELECT c.Date, c.PersonId, c.DayOff
		FROM inserted i
		INNER JOIN  dbo.v_PersonCalendar AS C ON i.PersonId = c.PersonId
		LEFT JOIN dbo.PersonCalendarAuto AS PCA ON C.PersonId = PCA.PersonId AND C.Date = PCA.Date
		WHERE PCA.Date IS NULL AND PCA.PersonId IS NULL

	END

END

GO

-- We need to specify the order of the trigger, because in tr_Person_Log trigger personHistory table needs to update and in tr_Person_UpdateCalendar we need to read the updated personHistory table rows.
EXEC sp_settriggerorder @triggername = N'[dbo].[tr_Person_UpdateCalendar]', @order = N'LAST', @stmttype = N'INSERT'
GO
EXEC sp_settriggerorder @triggername = N'[dbo].[tr_Person_UpdateCalendar]', @order = N'LAST', @stmttype = N'UPDATE'
GO
