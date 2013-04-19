CREATE TRIGGER [tr_ProjectGroup_Log]
    ON [dbo].ProjectGroup
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.GroupId AS BusinessUnitId,
				i.Name AS [BusinessUnit],
				i.Code ,
				i.ClientId AS AccountId,
				C.Name AS [Account] ,
				CASE WHEN i.Active = 1 THEN 'YES' ELSE 'NO' END [IsActive],
				i.BusinessGroupId,
				bg.Name AS BusinessGroup
		  FROM inserted AS i
		  INNER JOIN dbo.Client C ON C.ClientId = i.ClientId
		  INNER JOIN dbo.BusinessGroup bg ON bg.BusinessGroupId = i.BusinessGroupId
	),

	OLD_VALUES AS
	(
		SELECT d.GroupId AS BusinessUnitId,
				d.Name AS [BusinessUnit],
				d.Code ,
				d.ClientId AS AccountId,
				C.Name AS [Account] ,
				CASE WHEN d.Active = 1 THEN 'YES' ELSE 'NO' END [IsActive],
				d.BusinessGroupId,
				bg.Name AS BusinessGroup
		  FROM deleted AS d
		  INNER JOIN dbo.Client C ON C.ClientId = d.ClientId
		  INNER JOIN dbo.BusinessGroup bg ON bg.BusinessGroupId = d.BusinessGroupId
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
	           WHEN d.GroupId IS NULL THEN 3
	           WHEN i.GroupId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.BusinessUnitId = OLD_VALUES.BusinessUnitId
			           WHERE NEW_VALUES.BusinessUnitId = ISNULL(i.GroupId, d.GroupId) OR OLD_VALUES.BusinessUnitId = ISNULL(i.GroupId, d.GroupId)
					  FOR XML AUTO, ROOT('BusinessUnit'))),
		LogData = (SELECT 
						NEW_VALUES.BusinessUnitId 
						,NEW_VALUES.BusinessUnit
						,NEW_VALUES.AccountId
						,NEW_VALUES.Account
						,NEW_VALUES.Code
						,NEW_VALUES.IsActive
						,NEW_VALUES.BusinessGroupId
						,NEW_VALUES.BusinessGroup
						,OLD_VALUES.BusinessUnitId 
						,OLD_VALUES.BusinessUnit
						,OLD_VALUES.AccountId
						,OLD_VALUES.Account
						,OLD_VALUES.Code
						,OLD_VALUES.IsActive
						,OLD_VALUES.BusinessGroupId
						,OLD_VALUES.BusinessGroup
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.BusinessUnitId = OLD_VALUES.BusinessUnitId
			            WHERE NEW_VALUES.BusinessUnitId = ISNULL(i.GroupId, d.GroupId) OR OLD_VALUES.BusinessUnitId = ISNULL(i.GroupId, d.GroupId)
					FOR XML AUTO, ROOT('BusinessUnit'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.BusinessGroupId = d.BusinessGroupId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

