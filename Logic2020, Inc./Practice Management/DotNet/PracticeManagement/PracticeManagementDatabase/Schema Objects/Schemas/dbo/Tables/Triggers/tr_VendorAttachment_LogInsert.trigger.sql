CREATE TRIGGER [tr_VendorAttachment_LogInsert]
ON [dbo].[VendorAttachment]
AFTER INSERT
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	;WITH NEW_VALUES AS
	(
		SELECT i.Id
				,i.VendorId
				,v.Name as 'VendorName'
				,i.[FileName]
				,i.UploadedDate
		FROM inserted AS i
		LEFT JOIN Vendor v ON v.Id = i.VendorId
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
			           WHERE NEW_VALUES.Id = i.Id
					  FOR XML AUTO, ROOT('VendorAttachment'))),
			LogData = (SELECT *
					    FROM NEW_VALUES
			           WHERE NEW_VALUES.Id = i.Id
					  FOR XML AUTO, ROOT('VendorAttachment'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  
	  -- End logging session
	 EXEC dbo.SessionLogUnprepare
END
