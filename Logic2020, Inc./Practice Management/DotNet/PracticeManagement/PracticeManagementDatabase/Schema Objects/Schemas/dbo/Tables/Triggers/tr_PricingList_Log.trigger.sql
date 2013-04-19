CREATE TRIGGER [tr_PricingList_Log]
    ON [dbo].[PricingList]
    AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
   -- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.PricingListId ,
				i.Name AS PricingList,
				i.ClientId AS AccountId,
				c.Name AS [Account]
		  FROM inserted AS i
		  INNER JOIN dbo.Client C ON C.ClientId = i.ClientId
	),

	OLD_VALUES AS
	(
		SELECT d.PricingListId ,
				d.Name AS PricingList,
				d.ClientId AS AccountId,
				c.Name AS [Account]
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
	           WHEN d.PricingListId IS NULL THEN 3
	           WHEN i.PricingListId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PricingListId = OLD_VALUES.PricingListId 
			           WHERE NEW_VALUES.PricingListId = ISNULL(i.PricingListId, d.PricingListId) OR OLD_VALUES.PricingListId = ISNULL(i.PricingListId, d.PricingListId)
					  FOR XML AUTO, ROOT('PricingList'))),
		LogData = (SELECT 
						 NEW_VALUES.PricingListId 
						,NEW_VALUES.PricingList
						,NEW_VALUES.AccountId
						,NEW_VALUES.Account
						,OLD_VALUES.PricingListId
						,OLD_VALUES.PricingList
						,OLD_VALUES.AccountId
						,OLD_VALUES.Account
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PricingListId = OLD_VALUES.PricingListId
			           WHERE NEW_VALUES.PricingListId = ISNULL(i.PricingListId, d.PricingListId) OR OLD_VALUES.PricingListId = ISNULL(i.PricingListId, d.PricingListId)
					FOR XML AUTO, ROOT('PricingList'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.PricingListId = d.PricingListId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare


END

