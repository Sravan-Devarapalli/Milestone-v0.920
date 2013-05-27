CREATE TRIGGER [tr_Client_LogUpdateDelete]
    ON [dbo].[Client]
    FOR UPDATE,DELETE
AS 
BEGIN
  
    -- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT   i.ClientId AS AccountId
				,i.Name AS 'AccountName'
				,i.Code AS 'AccountCode'
				,i.DefaultDiscount
				,i.DefaultTerms
				,CASE WHEN i.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable'
				,CASE WHEN i.Inactive = 1 THEN 'No'
						ELSE 'Yes' END AS 'IsActive'
				,CASE WHEN i.IsInternal = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsInternal'
				,CASE WHEN i.IsHouseAccount = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsHouseAccount'
				,CASE WHEN i.IsNoteRequired = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsNoteRequired'
				,CASE WHEN i.IsMarginColorInfoEnabled = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsMarginColorInfoEnabled'
				,i.DefaultDirectorID
				,CASE WHEN dd.PersonId IS NULL THEN ''
						ELSE dd.LastName + ', ' + dd.FirstName END AS [DefaultDirector]	
				,i.DefaultSalespersonID	
				,sp.LastName + ', ' + sp.FirstName AS [DefaultSalesperson]			
		FROM inserted AS i
		INNER JOIN dbo.Person AS sp ON sp.PersonId = i.DefaultSalespersonID
		LEFT  JOIN dbo.Person AS dd ON dd.PersonId = i.DefaultDirectorID
	),

	OLD_VALUES AS
	(
		SELECT   d.ClientId AS AccountId
				,d.Name AS 'AccountName'
				,d.Code AS 'AccountCode'
				,d.DefaultDiscount
				,d.DefaultTerms
				,CASE WHEN d.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable'
				,CASE WHEN d.Inactive = 1 THEN 'No'
						ELSE 'Yes' END AS 'IsActive'
				,CASE WHEN d.IsInternal = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsInternal'
				,CASE WHEN d.IsHouseAccount = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsHouseAccount'
				,CASE WHEN d.IsNoteRequired = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsNoteRequired'
				,CASE WHEN d.IsMarginColorInfoEnabled = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsMarginColorInfoEnabled'
				,d.DefaultDirectorID
				,CASE WHEN dd.PersonId IS NULL THEN ''
						ELSE dd.LastName + ', ' + dd.FirstName END AS [DefaultDirector]	
				,d.DefaultSalespersonID	
				,sp.LastName + ', ' + sp.FirstName AS [DefaultSalesperson]			
		FROM deleted AS d
		INNER JOIN dbo.Person AS sp ON sp.PersonId = d.DefaultSalespersonID
		LEFT  JOIN dbo.Person AS dd ON dd.PersonId = d.DefaultDirectorID
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
	SELECT CASE WHEN i.ClientId IS NULL THEN 5
	            ELSE 4
	       END AS ActivityTypeID,
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.AccountId = OLD_VALUES.AccountId
			           WHERE NEW_VALUES.AccountId = ISNULL(i.ClientId, d.ClientId) OR OLD_VALUES.AccountId = ISNULL(i.ClientId, d.ClientId)
					  FOR XML AUTO, ROOT('Account'))),
			LogData = (SELECT 
							NEW_VALUES.AccountId,
							NEW_VALUES.AccountName,
							OLD_VALUES.AccountId,
							OLD_VALUES.AccountName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.AccountId = OLD_VALUES.AccountId
			           WHERE NEW_VALUES.AccountId = ISNULL(i.ClientId, d.ClientId) OR OLD_VALUES.AccountId = ISNULL(i.ClientId, d.ClientId)
					  FOR XML AUTO, ROOT('Account'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ClientId = d.ClientId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ClientId IS NULL -- deleted record
	    -- Detect changes
	    OR i.Name <> d.Name
	    OR i.Code <> d.Code
	    OR ISNULL(i.DefaultDirectorID, 0) <> ISNULL(d.DefaultDirectorID, 0)
	    OR i.DefaultDiscount <> d.DefaultDiscount
		OR i.DefaultSalespersonID <> d.DefaultSalespersonID
		OR i.DefaultTerms <> d.DefaultTerms
		OR i.Inactive <> d.Inactive
		OR i.IsChargeable <> d.IsChargeable
		OR i.IsInternal <> d.IsInternal
		OR i.IsMarginColorInfoEnabled <> d.IsMarginColorInfoEnabled
		OR i.IsNoteRequired <> d.IsNoteRequired
		OR i.IsHouseAccount <> d.IsHouseAccount

	-- End logging session
	 EXEC dbo.SessionLogUnprepare   

END

