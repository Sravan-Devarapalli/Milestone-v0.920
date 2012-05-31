-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 30-05-2012
-- Updated by:	ThulasiRam.P
-- Update date: 30-05-2012
-- Description:	Updates a PersonCalendarAuto table according to the data in the Calendar and PersonCalendar table
-- =============================================
CREATE TRIGGER [dbo].[tr_PersonCalendar_InsertUpdateDelete]
ON [dbo].[PersonCalendar]
AFTER INSERT, UPDATE , DELETE
AS
BEGIN
	SET NOCOUNT ON;

	 -- IF Delete Fires 
   
   IF NOT EXISTS (SELECT 1 FROM INSERTED)
   BEGIN
    
    UPDATE PCA
	SET PCA.DayOff = cal.DayOff
	FROM DELETED AS d
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON d.Date = PCA.Date  and d.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> cal.DayOff
     
   END
   -- IF UPDATE OR INSERT FIRES 
   ELSE 
   BEGIN

    UPDATE PCA
	SET PCA.DayOff = cal.DayOff
	FROM DELETED AS d
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON d.Date = PCA.Date  and d.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> cal.DayOff
   
    UPDATE PCA
	SET PCA.DayOff = i.DayOff
	FROM INSERTED AS i
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON i.Date = PCA.Date  and i.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> i.DayOff
	
   END


END

