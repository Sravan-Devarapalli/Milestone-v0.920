CREATE TRIGGER [tr_BusinessGroup_Log]
    ON [dbo].BusinessGroup
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.BusinessGroupId ,
				i.Name AS [BusinessGroup],
				i.Code ,
				i.ClientId AS AccountId,
				C.Name AS [Account] ,
				CASE WHEN i.Active = 1 THEN 'YES' ELSE 'NO' END [IsActive]
		  FROM inserted AS i
		  INNER JOIN dbo.Client C ON C.ClientId = i.ClientId
	),

	OLD_VALUES AS
	(
		SELECT d.BusinessGroupId ,
				d.Name AS [BusinessGroup],
				d.Code ,
				d.ClientId AS AccountId,
				C.Name AS [Account] ,
				CASE WHEN d.Active = 1 THEN 'YES' ELSE 'NO' END [IsActive]
		  FROM deleted AS d
		  INNER JOIN dbo.Client C ON C.ClientId = d.ClientId
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
	           WHEN d.BusinessGroupId IS NULL THEN 3
	           WHEN i.BusinessGroupId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.BusinessGroupId = OLD_VALUES.BusinessGroupId
			           WHERE NEW_VALUES.BusinessGroupId = ISNULL(i.BusinessGroupId, d.BusinessGroupId) OR OLD_VALUES.BusinessGroupId = ISNULL(i.BusinessGroupId, d.BusinessGroupId)
					  FOR XML AUTO, ROOT('BusinessGroup'))),
		LogData = (SELECT 
						NEW_VALUES.BusinessGroupId 
						,NEW_VALUES.BusinessGroup
						,NEW_VALUES.AccountId
						,NEW_VALUES.Account
						,NEW_VALUES.Code
						,NEW_VALUES.IsActive
						,OLD_VALUES.BusinessGroupId 
						,OLD_VALUES.BusinessGroup
						,OLD_VALUES.AccountId
						,OLD_VALUES.Account
						,OLD_VALUES.Code
						,OLD_VALUES.IsActive
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.BusinessGroupId = OLD_VALUES.BusinessGroupId
			            WHERE NEW_VALUES.BusinessGroupId = ISNULL(i.BusinessGroupId, d.BusinessGroupId) OR OLD_VALUES.BusinessGroupId = ISNULL(i.BusinessGroupId, d.BusinessGroupId)
					FOR XML AUTO, ROOT('BusinessGroup'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.BusinessGroupId = d.BusinessGroupId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

