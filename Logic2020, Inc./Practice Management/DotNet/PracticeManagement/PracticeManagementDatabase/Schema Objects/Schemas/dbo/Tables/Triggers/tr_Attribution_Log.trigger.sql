CREATE TRIGGER [tr_Attribution_Log]
		ON [dbo].[Attribution]
AFTER INSERT, UPDATE ,DELETE
AS 
    BEGIN
		-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
	SELECT	i.AttributionId,
			i.ProjectId,
			P.Name AS Project,
			i.AttributionTypeId,
			AT.Name AS AttributionType,
			i.AttributionRecordTypeId,
			ART.Name AS AttributionRecordType,
			CONVERT(NVARCHAR(10), i.StartDate, 101) AS [StartDate],
			CONVERT(NVARCHAR(10), i.EndDate, 101) AS [EndDate],
			i.Percentage AS CommissionPercentage,
			i.TargetId,
			CASE WHEN i.AttributionRecordTypeId = 1 THEN per.LastName+', '+per.FirstName ELSE pr.Name END AS TargetName
	FROM inserted AS i
	INNER JOIN dbo.Project P ON P.ProjectId = i.ProjectId
	INNER JOIN dbo.AttributionTypes AT ON AT.AttributionTypeId = i.AttributionTypeId
	INNER JOIN dbo.AttributionRecordTypes ART ON ART.AttributionRecordId = i.AttributionRecordTypeId
	LEFT JOIN dbo.Person per ON i.AttributionRecordTypeId = 1 AND per.PersonId = i.TargetId
	LEFT JOIN dbo.Practice pr ON i.AttributionRecordTypeId = 2 AND pr.PracticeId = i.TargetId
	),

	OLD_VALUES AS
	(
	SELECT	d.AttributionId,
			d.ProjectId,
			P.Name AS Project,
			d.AttributionTypeId,
			AT.Name AS AttributionType,
			d.AttributionRecordTypeId,
			ART.Name AS AttributionRecordType,
			CONVERT(NVARCHAR(10), d.StartDate, 101) AS [StartDate],
			CONVERT(NVARCHAR(10), d.EndDate, 101) AS [EndDate],
			d.Percentage AS CommissionPercentage,
			d.TargetId,
			CASE WHEN d.AttributionRecordTypeId = 1 THEN per.LastName+', '+per.FirstName ELSE pr.Name END AS TargetName
	FROM deleted AS d
	INNER JOIN dbo.Project P ON P.ProjectId = d.ProjectId
	INNER JOIN dbo.AttributionTypes AT ON AT.AttributionTypeId = d.AttributionTypeId
	INNER JOIN dbo.AttributionRecordTypes ART ON ART.AttributionRecordId = d.AttributionRecordTypeId
	LEFT JOIN dbo.Person per ON d.AttributionRecordTypeId = 1 AND per.PersonId = d.TargetId
	LEFT JOIN dbo.Practice pr ON d.AttributionRecordTypeId = 2 AND pr.PracticeId = d.TargetId
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
	           WHEN d.AttributionId IS NULL THEN 3
	           WHEN i.AttributionId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.AttributionId = OLD_VALUES.AttributionId 
			           WHERE NEW_VALUES.AttributionId = ISNULL(i.AttributionId, d.AttributionId) OR OLD_VALUES.AttributionId = ISNULL(i.AttributionId, d.AttributionId)
					  FOR XML AUTO, ROOT('Attribution'))),
		   LogData = (SELECT 
						 NEW_VALUES.AttributionId 
						,NEW_VALUES.ProjectId
						,NEW_VALUES.Project
						,NEW_VALUES.AttributionTypeId
						,NEW_VALUES.AttributionType
						,NEW_VALUES.AttributionRecordTypeId
						,NEW_VALUES.AttributionRecordType
						,NEW_VALUES.[StartDate] 
						,NEW_VALUES.[EndDate]
						,NEW_VALUES.CommissionPercentage
						,NEW_VALUES.TargetId 
						,NEW_VALUES.TargetName
						,OLD_VALUES.AttributionId 
						,OLD_VALUES.ProjectId
						,OLD_VALUES.Project
						,OLD_VALUES.AttributionTypeId
						,OLD_VALUES.AttributionType
						,OLD_VALUES.AttributionRecordTypeId
						,OLD_VALUES.AttributionRecordType
						,OLD_VALUES.[StartDate] 
						,OLD_VALUES.[EndDate]
						,OLD_VALUES.CommissionPercentage
						,OLD_VALUES.TargetId 
						,OLD_VALUES.TargetName
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.AttributionId = OLD_VALUES.AttributionId
			            WHERE NEW_VALUES.AttributionId = ISNULL(i.AttributionId , d.AttributionId ) OR OLD_VALUES.AttributionId = ISNULL(i.AttributionId , d.AttributionId)
					FOR XML AUTO, ROOT('Attribution'), TYPE),
					@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.AttributionId = d.AttributionId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
    END

