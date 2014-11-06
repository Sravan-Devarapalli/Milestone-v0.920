CREATE TRIGGER [tr_ProjectCapabilities_Log]
    ON [dbo].[ProjectCapabilities]
FOR DELETE, INSERT, UPDATE 
AS 
    BEGIN
    		-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
	SELECT	i.Id,
	        i.CapabilityId,
			PC.CapabilityName AS Capability,
			i.ProjectId,
			P.Name AS ProjectName
	FROM inserted AS i
	INNER JOIN dbo.Project P ON P.ProjectId = i.ProjectId
	INNER JOIN dbo.PracticeCapabilities PC ON PC.CapabilityId = i.CapabilityId
    WHERE P.ProjectId IS NOT NULL
	),

	OLD_VALUES AS
	(
	SELECT	d.Id,
			d.CapabilityId,
			PC.CapabilityName AS Capability,
			d.ProjectId,
			P.Name AS ProjectName
	FROM deleted AS d
	INNER JOIN dbo.Project P ON P.ProjectId = d.ProjectId
	INNER JOIN dbo.PracticeCapabilities PC ON PC.CapabilityId = d.CapabilityId
	WHERE P.ProjectId IS NOT NULL
	)

		-- Log an activity
		INSERT INTO dbo.UserActivityLogRecordPerChange
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
					 LogDate)
		SELECT  CASE
				   WHEN d.Id IS NULL THEN 3
				   WHEN i.Id IS NULL THEN 5
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
								 FULL JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id 
						   WHERE NEW_VALUES.Id = ISNULL(i.Id, d.Id) OR OLD_VALUES.Id = ISNULL(i.Id, d.Id)
						  FOR XML AUTO, ROOT('ProjectCapabilities'))),
						@CurrentPMTime
		  FROM inserted AS i
			   FULL JOIN deleted AS d ON i.Id = d.Id
			   INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
			   LEFT JOIN dbo.Project P ON P.ProjectId = d.ProjectId
			WHERE P.ProjectId IS NOT NULL
	
  END

