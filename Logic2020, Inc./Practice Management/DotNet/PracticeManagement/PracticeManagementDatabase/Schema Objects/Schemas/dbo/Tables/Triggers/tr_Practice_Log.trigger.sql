CREATE TRIGGER [tr_Practice_Log]
 ON  [dbo].[Practice]
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	;WITH NEW_VALUES AS
	(
		SELECT i.PracticeId,
				i.Name AS [Practice],
				i.Abbreviation,
				CASE WHEN i.IsActive = 1 THEN 'YES' ELSE 'NO' END [IsActive],
				CASE WHEN i.IsCompanyInternal = 1 THEN 'YES' ELSE 'NO' END [IsCompanyInternal],
				CASE WHEN i.IsNotesRequired = 1 THEN 'YES' ELSE 'NO' END [IsNotesRequired],
				p.PersonId as [PracticeManagerId],
				p.LastName + ', ' + p.FirstName as [PracticeManager]
		  FROM inserted AS i
		  INNER JOIN dbo.Person P ON P.PersonId = i.PracticeManagerId
	),

	OLD_VALUES AS
	(
			SELECT d.PracticeId,
				d.Name AS [Practice],
				d.Abbreviation,
				CASE WHEN d.IsActive = 1 THEN 'YES' ELSE 'NO' END [IsActive],
				CASE WHEN d.IsCompanyInternal = 1 THEN 'YES' ELSE 'NO' END [IsCompanyInternal],
				CASE WHEN d.IsNotesRequired = 1 THEN 'YES' ELSE 'NO' END [IsNotesRequired],
				p.PersonId as [PracticeManagerId],
				p.LastName + ', ' + p.FirstName as [PracticeManager]
		  FROM deleted AS d
		  INNER JOIN dbo.Person P ON P.PersonId = d.PracticeManagerId
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
	           WHEN d.PracticeId IS NULL THEN 3
	           WHEN i.PracticeId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PracticeId = OLD_VALUES.PracticeId
			           WHERE NEW_VALUES.PracticeId = ISNULL(i.PracticeId, d.PracticeId) OR OLD_VALUES.PracticeId = ISNULL(i.PracticeId, d.PracticeId)
					  FOR XML AUTO, ROOT('Practice'))),
		LogData = (SELECT 
						NEW_VALUES.PracticeId 
						,NEW_VALUES.Practice
						,NEW_VALUES.Abbreviation
						,NEW_VALUES.IsActive
						,NEW_VALUES.IsCompanyInternal
						,NEW_VALUES.IsNotesRequired
						,NEW_VALUES.PracticeManagerId
						,NEW_VALUES.[PracticeManager]
						,OLD_VALUES.PracticeId 
						,OLD_VALUES.Practice
						,OLD_VALUES.Abbreviation
						,OLD_VALUES.IsActive
						,OLD_VALUES.IsCompanyInternal
						,OLD_VALUES.IsNotesRequired
						,OLD_VALUES.PracticeManagerId
						,OLD_VALUES.[PracticeManager]
					 FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PracticeId = OLD_VALUES.PracticeId
			           WHERE NEW_VALUES.PracticeId = ISNULL(i.PracticeId, d.PracticeId) OR OLD_VALUES.PracticeId = ISNULL(i.PracticeId, d.PracticeId)
					FOR XML AUTO, ROOT('Practice'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.PracticeId = d.PracticeId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	
	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END
