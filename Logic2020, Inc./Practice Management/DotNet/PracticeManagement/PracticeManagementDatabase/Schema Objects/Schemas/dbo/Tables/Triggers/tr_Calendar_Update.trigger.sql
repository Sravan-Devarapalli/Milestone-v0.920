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

-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	UPDATE PCA
	SET PCA.DayOff = ISNULL(pcal.DayOff, i.DayOff),
		PCA.CompanyDayOff = i.DayOff,
		PCA.TimeOffHours = pcal.ActualHours
	FROM dbo.PersonCalendarAuto AS PCA
	INNER JOIN INSERTED AS i ON i.Date = PCA.Date
	INNER JOIN dbo.Person AS p ON  p.PersonId = PCA.PersonId
	LEFT JOIN dbo.PersonCalendar AS pcal ON pcal.Date = i.Date AND pcal.PersonId = p.PersonId AND PCA.PersonId = pcal.PersonId
	WHERE PCA.DayOff <> ISNULL(pcal.DayOff, i.DayOff) OR PCA.CompanyDayOff <> i.DayOff
		 
		EXEC SessionLogPrepare @UserLogin = NULL
	
	;WITH NEW_VALUES AS
	(
	SELECT  Date,
			DayOff,
			CASE WHEN Count > 1 THEN 'YES' ELSE 'NO' END IsRecurring,
			HolidayDescription
	FROM 
	(
	SELECT	ROW_NUMBER() OVER(ORDER BY i.DATE) AS RowNumber,
			CONVERT(NVARCHAR(10), i.Date, 101) AS Date,
			CASE WHEN i.DayOff = 1 THEN 'YES' ELSE 'NO' END AS DayOff,
			(SELECT COUNT(*) FROM inserted) AS Count,
			i.HolidayDescription
	FROM inserted AS i
	INNER JOIN deleted  AS d on d.Date = i.Date AND i.DayOff <> d.DayOff AND i.DayOff = 1 
	WHERE DATEPART(DW,i.Date) NOT IN (1,7) 
	) as A
	where a.RowNumber = 1
	),

	OLD_VALUES AS
	(
	SELECT  Date,
			DayOff,
			CASE WHEN Count > 1 THEN 'YES' ELSE 'NO' END IsRecurring,
			HolidayDescription
	FROM 
	(
	SELECT	ROW_NUMBER() OVER(ORDER BY D.DATE) AS RowNumber,
			CONVERT(NVARCHAR(10), d.Date, 101) AS Date,
			CASE WHEN d.DayOff = 1 THEN 'YES' ELSE 'NO' END AS DayOff,
			(SELECT COUNT(*) FROM deleted) AS Count,
			d.HolidayDescription
	FROM deleted AS d
	INNER JOIN inserted AS i on d.Date = i.Date AND i.DayOff <> d.DayOff AND d.DayOff = 1 
	WHERE DATEPART(DW,i.Date) NOT IN (1,7) 
	) as A
	where a.RowNumber = 1
	)

	-- Log an activity
	INSERT INTO dbo.UserActivityLog
	            (ActivityTypeID,
	             SessionID,
	             SystemUser,
	             Workstation,
	             ApplicationName,
	             UserLogin,
	             PersonID,
	             LastName,
	             FirstName,
				 Data,
	             LogData,
	             LogDate)
	SELECT  CASE
	           WHEN d.Date IS NULL THEN 3
			   WHEN i.Date IS NULL THEN 5
	           ELSE 4
	       END as ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Date = OLD_VALUES.Date 
			           WHERE NEW_VALUES.Date = ISNULL(i.Date, d.Date) OR OLD_VALUES.Date = ISNULL(i.Date, d.Date)
					  FOR XML AUTO, ROOT('CompanyHoliday'))),
		   LogData = (SELECT 
						 NEW_VALUES.Date 
						,NEW_VALUES.DayOff
						,NEW_VALUES.IsRecurring
						,NEW_VALUES.HolidayDescription

						,OLD_VALUES.Date 
						,OLD_VALUES.DayOff
						,OLD_VALUES.IsRecurring
						,OLD_VALUES.HolidayDescription

						FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Date = OLD_VALUES.Date
			            WHERE NEW_VALUES.Date = ISNULL(i.Date , d.Date ) OR OLD_VALUES.Date = ISNULL(i.Date , d.Date)
					FOR XML AUTO, ROOT('CompanyHoliday'), TYPE),
					@CurrentPMTime
	  FROM NEW_VALUES AS i
	       FULL JOIN OLD_VALUES AS d ON i.Date = d.Date
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 --TO LOG COMPANY WORKING DAYS
	 
	;WITH NEW_VALUES AS
	(
	SELECT	CONVERT(NVARCHAR(10), i.Date, 101) AS Date
	FROM inserted AS i
	INNER JOIN deleted  AS d on d.Date = i.Date AND i.DayOff <> d.DayOff AND i.DayOff = 0 
	WHERE DATEPART(DW,i.Date) IN (1,7) 
	),

	OLD_VALUES AS
	(
	SELECT	CONVERT(NVARCHAR(10), d.Date, 101) AS Date
	FROM inserted AS i
	INNER JOIN deleted  AS d on d.Date = i.Date AND i.DayOff <> d.DayOff AND d.DayOff = 0 
	WHERE DATEPART(DW,i.Date) IN (1,7)   
	)

	-- Log an activity
	INSERT INTO dbo.UserActivityLog
	            (ActivityTypeID,
	             SessionID,
	             SystemUser,
	             Workstation,
	             ApplicationName,
	             UserLogin,
	             PersonID,
	             LastName,
	             FirstName,
				 Data,
	             LogData,
	             LogDate)
	SELECT  CASE
	           WHEN d.Date IS NULL THEN 3
			   WHEN i.Date IS NULL THEN 5
	           ELSE 4
	       END as ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Date = OLD_VALUES.Date 
			           WHERE NEW_VALUES.Date = ISNULL(i.Date, d.Date) OR OLD_VALUES.Date = ISNULL(i.Date, d.Date)
					  FOR XML AUTO, ROOT('CompanyWorkingDay'))),
		   LogData = (SELECT 
						 NEW_VALUES.Date 
						,OLD_VALUES.Date 
						FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Date = OLD_VALUES.Date
			            WHERE NEW_VALUES.Date = ISNULL(i.Date , d.Date ) OR OLD_VALUES.Date = ISNULL(i.Date , d.Date)
					FOR XML AUTO, ROOT('CompanyWorkingDay'), TYPE),
					@CurrentPMTime
	  FROM NEW_VALUES AS i
	       FULL JOIN OLD_VALUES AS d ON i.Date = d.Date
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	-- End logging session
	 EXEC dbo.SessionLogUnprepare
END

