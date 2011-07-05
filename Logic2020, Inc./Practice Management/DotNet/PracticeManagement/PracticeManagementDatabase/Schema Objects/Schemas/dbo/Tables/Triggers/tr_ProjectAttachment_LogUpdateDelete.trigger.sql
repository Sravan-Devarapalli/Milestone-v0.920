CREATE TRIGGER [dbo].[tr_ProjectAttachment_LogUpdateDelete]
ON [dbo].[ProjectAttachment]
AFTER UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.Id,i.ProjectId,i.UploadedDate,i.[FileName]
		  FROM inserted AS i
	),

	OLD_VALUES AS
	(
		SELECT d.Id,d.ProjectId,d.UploadedDate,d.[FileName]
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
	           WHEN d.Id IS NULL THEN 3 -- Actually is redundant
	           WHEN i.Id IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id
			           WHERE NEW_VALUES.Id = ISNULL(i.Id, d.Id) OR OLD_VALUES.Id = ISNULL(i.Id, d.Id)
					  FOR XML AUTO, ROOT('ProjectAttachment'))),
			LogData = (SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id
			           WHERE NEW_VALUES.Id = ISNULL(i.Id, d.Id) OR OLD_VALUES.Id = ISNULL(i.Id, d.Id)
					  FOR XML AUTO, ROOT('ProjectAttachment'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.Id = d.Id
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.Id IS NULL -- deleted record
	    -- Detect changes
	    OR i.[FileName] <> d.[FileName]
	    OR i.UploadedDate <> d.UploadedDate
	-- End logging session
	 EXEC dbo.SessionLogUnprepare
END
GO
