CREATE TRIGGER [dbo].[tr_ProjectAttachment_LogInsert]
ON [dbo].[ProjectAttachment]
AFTER INSERT
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

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
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.Id,NEW_VALUES.ProjectId, NEW_VALUES.[FileName],NEW_VALUES.UploadedDate
					    FROM inserted AS NEW_VALUES
			           WHERE NEW_VALUES.Id = i.Id
					  FOR XML AUTO, ROOT('ProjectAttachment'))),
			LogData = (SELECT NEW_VALUES.Id,NEW_VALUES.ProjectId, NEW_VALUES.[FileName],NEW_VALUES.UploadedDate
					    FROM inserted AS NEW_VALUES
			           WHERE NEW_VALUES.Id = i.Id
					  FOR XML AUTO, ROOT('ProjectAttachment'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  
	  -- End logging session
	 EXEC dbo.SessionLogUnprepare
END
GO
