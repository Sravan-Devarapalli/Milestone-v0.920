CREATE TRIGGER [tr_ProjectFeedback]
    ON [dbo].[ProjectFeedback]
AFTER INSERT, UPDATE ,DELETE
AS 
    BEGIN
		
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	;WITH NEW_VALUES AS
	(
	SELECT	i.FeedbackId,
			i.ProjectId,
			P.Name AS Project,
			i.PersonId,
			person.LastName+', '+person.FirstName AS ResourceName,
			CONVERT(NVARCHAR(10), i.ReviewPeriodStartDate, 101) AS [ReviewPeriodStartDate],
			CONVERT(NVARCHAR(10), i.ReviewPeriodEndDate, 101) AS [ReviewPeriodEndDate],
			CONVERT(NVARCHAR(10), i.DueDate, 101) AS [FeedbackDueDate],
			CASE WHEN i.IsCanceled = 1 THEN 'YES' ELSE 'NO' END AS [IsFeedbackCanceled],
			i.FeedbackStatusId,
			PFS.Name AS FeedbackStatus,
			CONVERT(NVARCHAR(10), i.CompletionCertificateDate, 101) AS [StatusUpdatedDate],
			i.CompletionCertificateBy AS StatusUpdatedById,
			StatusUpdate.LastName+', '+StatusUpdate.FirstName AS [StatusUpdatedBy],
			i.CancelationReason,
			CASE WHEN i.IsGap = 1 THEN 'YES' ELSE 'NO' END AS [IsGap]
	FROM inserted AS i
	INNER JOIN dbo.Project P ON P.ProjectId = i.ProjectId
	INNER JOIN dbo.Person person ON person.PersonId = i.PersonId 
	INNER JOIN dbo.ProjectFeedbackStatus PFS ON PFS.FeedbackStatusId = i.FeedbackStatusId
	LEFT JOIN dbo.Person StatusUpdate ON StatusUpdate.PersonId = i.CompletionCertificateBy
	),

	OLD_VALUES AS
	(
	SELECT	d.FeedbackId,
			d.ProjectId,
			P.Name AS Project,
			d.PersonId,
			person.LastName+', '+person.FirstName AS ResourceName,
			CONVERT(NVARCHAR(10), d.ReviewPeriodStartDate, 101) AS [ReviewPeriodStartDate],
			CONVERT(NVARCHAR(10), d.ReviewPeriodEndDate, 101) AS [ReviewPeriodEndDate],
			CONVERT(NVARCHAR(10), d.DueDate, 101) AS [FeedbackDueDate],
			CASE WHEN d.IsCanceled = 1 THEN 'YES' ELSE 'NO' END AS [IsFeedbackCanceled],
			d.FeedbackStatusId,
			PFS.Name AS FeedbackStatus,
			CONVERT(NVARCHAR(10), d.CompletionCertificateDate, 101) AS [StatusUpdatedDate],
			d.CompletionCertificateBy AS StatusUpdatedById,
			StatusUpdate.LastName+', '+StatusUpdate.FirstName AS [StatusUpdatedBy],
			d.CancelationReason,
			CASE WHEN d.IsGap = 1 THEN 'YES' ELSE 'NO' END AS [IsGap]
	FROM deleted AS d
	INNER JOIN dbo.Project P ON P.ProjectId = d.ProjectId
	INNER JOIN dbo.Person person ON person.PersonId = d.PersonId 
	INNER JOIN dbo.ProjectFeedbackStatus PFS ON PFS.FeedbackStatusId = d.FeedbackStatusId
	LEFT JOIN dbo.Person StatusUpdate ON StatusUpdate.PersonId = d.CompletionCertificateBy
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
	           WHEN d.FeedbackId IS NULL THEN 3
	           WHEN i.FeedbackId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.FeedbackId = OLD_VALUES.FeedbackId 
			           WHERE NEW_VALUES.FeedbackId = ISNULL(i.FeedbackId, d.FeedbackId) OR OLD_VALUES.FeedbackId = ISNULL(i.FeedbackId, d.FeedbackId)
					  FOR XML AUTO, ROOT('ProjectFeedback'))),
		   LogData = (SELECT 
						 NEW_VALUES.FeedbackId 
						,NEW_VALUES.ProjectId
						,NEW_VALUES.Project
						,NEW_VALUES.PersonId
						,NEW_VALUES.ResourceName
						,NEW_VALUES.ReviewPeriodStartDate
						,NEW_VALUES.ReviewPeriodEndDate
						,NEW_VALUES.[FeedbackDueDate] 
						,NEW_VALUES.[IsFeedbackCanceled]
						,NEW_VALUES.FeedbackStatusId
						,NEW_VALUES.FeedbackStatus 
						,NEW_VALUES.[StatusUpdatedDate]
						,NEW_VALUES.StatusUpdatedById
						,NEW_VALUES.[StatusUpdatedBy]
						,NEW_VALUES.CancelationReason 
						,NEW_VALUES.IsGap

						,OLD_VALUES.FeedbackId 
						,OLD_VALUES.ProjectId
						,OLD_VALUES.Project
						,OLD_VALUES.PersonId
						,OLD_VALUES.ResourceName
						,OLD_VALUES.ReviewPeriodStartDate
						,OLD_VALUES.ReviewPeriodEndDate
						,OLD_VALUES.[FeedbackDueDate] 
						,OLD_VALUES.[IsFeedbackCanceled]
						,OLD_VALUES.FeedbackStatusId
						,OLD_VALUES.FeedbackStatus 
						,OLD_VALUES.[StatusUpdatedDate]
						,OLD_VALUES.StatusUpdatedById
						,OLD_VALUES.[StatusUpdatedBy]
						,OLD_VALUES.CancelationReason 
						,OLD_VALUES.IsGap
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.FeedbackId = OLD_VALUES.FeedbackId
			            WHERE NEW_VALUES.FeedbackId = ISNULL(i.FeedbackId , d.FeedbackId ) OR OLD_VALUES.FeedbackId = ISNULL(i.FeedbackId , d.FeedbackId)
					FOR XML AUTO, ROOT('ProjectFeedback'), TYPE),
					@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.FeedbackId = d.FeedbackId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	-- End logging session
	 EXEC dbo.SessionLogUnprepare

    END

