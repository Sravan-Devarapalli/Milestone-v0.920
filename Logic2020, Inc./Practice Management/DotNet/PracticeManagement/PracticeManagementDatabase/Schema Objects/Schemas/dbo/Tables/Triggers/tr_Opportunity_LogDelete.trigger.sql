CREATE TRIGGER [dbo].[tr_Opportunity_LogDelete]
ON [dbo].[Opportunity]
AFTER  DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.*
		  FROM inserted AS i
	),

	OLD_VALUES AS
	(
		SELECT d.*
		  FROM deleted AS d
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
	SELECT CASE
	           WHEN d.OpportunityId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.OpportunityId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityId = OLD_VALUES.OpportunityId
			           WHERE NEW_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId) OR OLD_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId)
					  FOR XML AUTO, ROOT('Opportunity'))),
			LogData = (SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityId = OLD_VALUES.OpportunityId
			           WHERE NEW_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId) OR OLD_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId)
					  FOR XML AUTO, ROOT('Opportunity'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.OpportunityId = d.OpportunityId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.OpportunityId IS NULL -- deleted record
	    
	-- End logging session
	 EXEC dbo.SessionLogUnprepare
END


