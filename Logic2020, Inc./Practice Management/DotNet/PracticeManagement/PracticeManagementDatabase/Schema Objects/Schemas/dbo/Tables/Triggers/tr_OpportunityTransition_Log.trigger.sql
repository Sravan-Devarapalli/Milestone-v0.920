CREATE TRIGGER tr_OpportunityTransition_Log 
   ON  dbo.[OpportunityTransition] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	;WITH NEW_VALUES AS
	(
		SELECT i.[OpportunityTransitionId]
			   ,i.[OpportunityTransitionStatusId]
			   ,i.[TransitionDate]
			   ,i.[PersonId]
			   ,i.[NoteText]
			   ,i.OpportunityId
			   ,i.TargetPersonId 
			   ,opp.[Name] as 'OpportunityName'
			   ,pers.LastName + ', ' + pers.FirstName as 'Person'
			   ,transitionStatus.Name as 'TransitionType'
		  FROM inserted AS i
		       INNER JOIN Opportunity as opp ON i.OpportunityId = opp.OpportunityId
			   inner join person as pers on pers.PersonId = i.TargetPersonId
			   inner join OpportunityTransitionStatus as transitionStatus on transitionStatus.OpportunityTransitionStatusId = i.OpportunityTransitionStatusId
	),

	OLD_VALUES AS
	(
		SELECT d.[OpportunityTransitionId]
			   ,d.[OpportunityTransitionStatusId]
			   ,d.[TransitionDate]
			   ,d.[PersonId]
			   ,d.[NoteText]
			   ,d.OpportunityId 
			   ,d.TargetPersonId 
			   ,opp.[Name] as 'OpportunityName'
			   ,pers.LastName + ', ' + pers.FirstName as 'Person'
			   ,transitionStatus.Name as 'TransitionType'
		  FROM deleted AS d
		       INNER JOIN Opportunity as opp ON d.OpportunityId = opp.OpportunityId
			   inner join person as pers on pers.PersonId = d.TargetPersonId
			   inner join OpportunityTransitionStatus as transitionStatus on transitionStatus.OpportunityTransitionStatusId = d.OpportunityTransitionStatusId
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
	             LogData)
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
							FOR XML AUTO, ROOT('OpportunityTransition'), TYPE)
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.OpportunityTransitionId = d.OpportunityTransitionId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare

END
