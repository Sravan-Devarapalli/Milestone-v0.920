CREATE TRIGGER [tr_PracticeCapabilities_Log]
    ON [dbo].PracticeCapabilities
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.CapabilityId AS [PracticeCapabilityId],
				P.Name AS [PracticeArea],
				i.CapabilityName AS [PracticeCapability],
				i.PracticeId,
				CASE WHEN i.IsActive = 1 THEN 'YES' ELSE 'NO' END [IsActive]
		  FROM inserted AS i
		  INNER JOIN dbo.Practice P ON P.PracticeId = i.PracticeId
	),

	OLD_VALUES AS
	(
		SELECT d.CapabilityId AS [PracticeCapabilityId],
				P.Name AS [PracticeArea],
				d.CapabilityName AS [PracticeCapability],
				d.PracticeId,
				CASE WHEN d.IsActive = 1 THEN 'YES' ELSE 'NO' END [IsActive]
		  FROM deleted AS d
		  INNER JOIN dbo.Practice P ON P.PracticeId = d.PracticeId
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
	           WHEN d.CapabilityId IS NULL THEN 3
	           WHEN i.CapabilityId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.[PracticeCapabilityId] = OLD_VALUES.[PracticeCapabilityId]
			           WHERE NEW_VALUES.[PracticeCapabilityId] = ISNULL(i.CapabilityId, d.CapabilityId) OR OLD_VALUES.[PracticeCapabilityId] = ISNULL(i.CapabilityId, d.CapabilityId)
					  FOR XML AUTO, ROOT('PracticeCapability'))),
		LogData = (SELECT 
						NEW_VALUES.PracticeCapabilityId 
						,NEW_VALUES.[PracticeArea]
						,NEW_VALUES.PracticeCapability
						,NEW_VALUES.PracticeId
						,NEW_VALUES.IsActive
						,OLD_VALUES.PracticeCapabilityId 
						,OLD_VALUES.[PracticeArea]
						,OLD_VALUES.PracticeCapability
						,OLD_VALUES.PracticeId
						,OLD_VALUES.IsActive
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.[PracticeCapabilityId] = OLD_VALUES.[PracticeCapabilityId]
			           WHERE NEW_VALUES.[PracticeCapabilityId] = ISNULL(i.CapabilityId, d.CapabilityId) OR OLD_VALUES.[PracticeCapabilityId] = ISNULL(i.CapabilityId, d.CapabilityId)
					FOR XML AUTO, ROOT('PracticeCapability'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.PracticeId = d.PracticeId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

