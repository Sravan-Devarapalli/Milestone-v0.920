-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-25-2008
-- Updated by:	
-- Update date:	
-- Description:	Logs the inserting into the dbo.Project table.
-- =============================================
CREATE TRIGGER [dbo].[tr_Project_LogInsert]
ON [dbo].[Project]
AFTER INSERT
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @GMT NVARCHAR(10) = (SELECT Value FROM Settings WHERE SettingsKey = 'TimeZone')
	DECLARE @CurrentPMTime DATETIME = (CASE WHEN CHARINDEX('-',@GMT) >0 THEN GETUTCDATE() - REPLACE(@GMT,'-','') ELSE 
											GETUTCDATE() + @GMT END)

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
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId, NEW_VALUES.Name
					    FROM inserted AS NEW_VALUES
			           WHERE NEW_VALUES.ProjectId = i.ProjectId
					  FOR XML AUTO, ROOT('Project'))),
			LogData = (SELECT NEW_VALUES.ProjectId
					    FROM inserted AS NEW_VALUES
			           WHERE NEW_VALUES.ProjectId = i.ProjectId
					  FOR XML AUTO, ROOT('Project'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  
	  -- End logging session
	 EXEC dbo.SessionLogUnprepare
END


GO



