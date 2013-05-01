CREATE TRIGGER [tr_ProjectCSAT_Log]
    ON [dbo].[ProjectCSAT]
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.CSATId ,
				CONVERT(NVARCHAR(10), i.ReviewStartDate, 101) AS [ReviewStartDate],
				CONVERT(NVARCHAR(10), i.ReviewEndDate, 101) AS [ReviewEndDate],	
				CONVERT(NVARCHAR(10), i.CompletionDate, 101) AS [CompletionDate],
				i.ProjectId,
				p.Name AS [Project] ,
				i.Comments,
				i.ReferralScore,
				i.ReviewerId,
				per.LastName +', '+ per.FirstName AS Reviewer
		  FROM inserted AS i
		  INNER JOIN dbo.Project p ON p.ProjectId = i.ProjectId
		  INNER JOIN dbo.Person per ON per.PersonId = i.ReviewerId
	),

	OLD_VALUES AS
	(
		SELECT	d.CSATId ,
				CONVERT(NVARCHAR(10), d.ReviewStartDate, 101) AS [ReviewStartDate],
				CONVERT(NVARCHAR(10), d.ReviewEndDate, 101) AS [ReviewEndDate],	
				CONVERT(NVARCHAR(10), d.CompletionDate, 101) AS [CompletionDate],
				d.ProjectId,
				p.Name AS [Project] ,
				d.Comments,
				d.ReferralScore,
				d.ReviewerId,
				per.LastName +', '+ per.FirstName AS Reviewer
		  FROM deleted AS d
		  INNER JOIN dbo.Project p ON p.ProjectId = d.ProjectId
		  INNER JOIN dbo.Person per ON per.PersonId = d.ReviewerId
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
	           WHEN d.CSATId IS NULL THEN 3
	           WHEN i.CSATId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.CSATId = OLD_VALUES.CSATId
			           WHERE NEW_VALUES.CSATId = ISNULL(i.CSATId, d.CSATId) OR OLD_VALUES.CSATId = ISNULL(i.CSATId, d.CSATId)
					  FOR XML AUTO, ROOT('ProjectCSAT'))),
		LogData = (SELECT 
						NEW_VALUES.CSATId 
						,NEW_VALUES.ReviewStartDate
						,NEW_VALUES.ReviewEndDate
						,NEW_VALUES.CompletionDate
						,NEW_VALUES.ProjectId
						,NEW_VALUES.[Project]
						,NEW_VALUES.Comments
						,NEW_VALUES.ReferralScore 
						,NEW_VALUES.ReviewerId
						,NEW_VALUES.Reviewer
						,OLD_VALUES.CSATId 
						,OLD_VALUES.ReviewStartDate
						,OLD_VALUES.ReviewEndDate
						,OLD_VALUES.CompletionDate
						,OLD_VALUES.ProjectId
						,OLD_VALUES.[Project]
						,OLD_VALUES.Comments
						,OLD_VALUES.ReferralScore 
						,OLD_VALUES.ReviewerId
						,OLD_VALUES.Reviewer
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.CSATId = OLD_VALUES.CSATId
			            WHERE NEW_VALUES.CSATId = ISNULL(i.CSATId, d.CSATId) OR OLD_VALUES.CSATId = ISNULL(i.CSATId, d.CSATId)
					FOR XML AUTO, ROOT('ProjectCSAT'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.CSATId = d.CSATId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

