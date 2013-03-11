-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 30-05-2012
-- Updated by:	ThulasiRam.P
-- Update date: 30-05-2012
-- Description:	Updates a PersonCalendarAuto table according to the data in the Calendar and PersonCalendar table
-- =============================================
CREATE TRIGGER [dbo].[tr_Calendar_Update]
ON [dbo].[Calendar]
AFTER  UPDATE
AS
BEGIN
SET NOCOUNT ON;

	UPDATE PCA
	SET PCA.DayOff = ISNULL(pcal.DayOff, i.DayOff),
		PCA.CompanyDayOff = i.DayOff,
		PCA.TimeOffHours = pcal.ActualHours
	FROM dbo.PersonCalendarAuto AS PCA
	INNER JOIN INSERTED AS i ON i.Date = PCA.Date
	INNER JOIN dbo.Person AS p ON  p.PersonId = PCA.PersonId
	LEFT JOIN dbo.PersonCalendar AS pcal ON pcal.Date = i.Date AND pcal.PersonId = p.PersonId AND PCA.PersonId = pcal.PersonId
	WHERE PCA.DayOff <> ISNULL(pcal.DayOff, i.DayOff) OR PCA.CompanyDayOff <> i.DayOff

END

