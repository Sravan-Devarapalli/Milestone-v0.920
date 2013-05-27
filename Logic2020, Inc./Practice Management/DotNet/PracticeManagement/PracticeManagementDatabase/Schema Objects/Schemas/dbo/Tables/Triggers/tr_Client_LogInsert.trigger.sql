CREATE TRIGGER [tr_Client_LogInsert]
    ON [dbo].[Client]
    FOR INSERT
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
	SELECT 3 AS ActivityTypeID /* insert only */,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT *
										FROM NEW_VALUES
										WHERE NEW_VALUES.AccountId = i.ClientId
					  FOR XML AUTO, ROOT('Account'))),
			LogData = (SELECT ins.ClientId AS AccountId
					    FROM inserted AS ins
			           WHERE ins.ClientId = i.ClientId
					  FOR XML AUTO, ROOT('Account'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  
	  -- End logging session
	 EXEC dbo.SessionLogUnprepare

END

