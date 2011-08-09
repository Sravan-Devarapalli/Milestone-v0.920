CREATE TRIGGER [dbo].[tr_OpportunityTransition_Log] 
   ON  [dbo].[OpportunityTransition] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.[OpportunityTransitionId]
			   ,i.[OpportunityTransitionStatusId]
			   ,transitionStatus.Name AS 'TransitionType'
			   ,i.[TransitionDate]
			   ,i.[PersonId] --Modified person
			   ,i.[NoteText]
			   ,i.OpportunityId
			   ,opp.[Name] as 'OpportunityName'
			   ,i.TargetPersonId 
			   ,pers.LastName + ', ' + pers.FirstName as 'Person'
		  FROM inserted AS i
		       INNER JOIN Opportunity as opp ON i.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityTransitionStatus transitionStatus ON i.OpportunityTransitionStatusId  = transitionStatus.OpportunityTransitionStatusId
		       INNER JOIN Person as p ON p.PersonId = i.PersonId
			   LEFT JOIN person as pers on pers.PersonId = i.TargetPersonId
	),

	OLD_VALUES AS
	(
		SELECT d.[OpportunityTransitionId]
			   ,d.[OpportunityTransitionStatusId]
			   ,transitionStatus.Name AS 'TransitionType'
			   ,d.[TransitionDate]
			   ,d.[PersonId] --Modified person
			   ,d.[NoteText]
			   ,d.OpportunityId
			   ,opp.[Name] as 'OpportunityName'
			   ,d.TargetPersonId 
			   ,pers.LastName + ', ' + pers.FirstName as 'Person'
		  FROM deleted AS d
		       INNER JOIN Opportunity as opp ON d.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityTransitionStatus transitionStatus ON d.OpportunityTransitionStatusId  = transitionStatus.OpportunityTransitionStatusId
		       INNER JOIN Person as p ON p.PersonId = d.PersonId
			   LEFT JOIN person as pers on pers.PersonId = d.TargetPersonId
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
	           WHEN d.OpportunityTransitionId IS NULL THEN 3
	           WHEN i.OpportunityTransitionId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityTransitionId = OLD_VALUES.OpportunityTransitionId
			           WHERE NEW_VALUES.OpportunityTransitionId = ISNULL(i.OpportunityTransitionId, d.OpportunityTransitionId) OR OLD_VALUES.OpportunityTransitionId = ISNULL(i.OpportunityTransitionId, d.OpportunityTransitionId)
					  FOR XML AUTO, ROOT('OpportunityTransition'))),
			LogData = (SELECT 
							NEW_VALUES.[OpportunityTransitionId]
							,NEW_VALUES.[OpportunityTransitionStatusId]
							,NEW_VALUES.[TransitionDate]
							,NEW_VALUES.[PersonId]
							,NEW_VALUES.OpportunityId
							,NEW_VALUES.TargetPersonId 
							,OLD_VALUES.[OpportunityTransitionId]
							,OLD_VALUES.[OpportunityTransitionStatusId]
							,OLD_VALUES.[TransitionDate]
							,OLD_VALUES.[PersonId]
							,OLD_VALUES.OpportunityId
							,OLD_VALUES.TargetPersonId 
							
							FROM NEW_VALUES
									FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityTransitionId = OLD_VALUES.OpportunityTransitionId
							WHERE NEW_VALUES.OpportunityTransitionId = ISNULL(i.OpportunityTransitionId, d.OpportunityTransitionId) OR OLD_VALUES.OpportunityTransitionId = ISNULL(i.OpportunityTransitionId, d.OpportunityTransitionId)
							FOR XML AUTO, ROOT('OpportunityTransition'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.OpportunityTransitionId = d.OpportunityTransitionId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare

END



