CREATE TRIGGER [Skills].[tr_PersonSkill_LogInsert]
ON [Skills].[PersonSkill]
FOR INSERT
AS 
BEGIN
    SET NOCOUNT ON

	
	-- Ensure the temporary table exists
	--EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	;WITH NEW_VALUES AS
	(
		SELECT i.*,
				S.Description 'SkillsDescription',
				SL.Description 'SkillLevel',
				P.LastName + ', ' + P.FirstName [Person]
		  FROM inserted AS i
			   INNER JOIN Skills.Skill AS S ON S.SkillId = i.SkillId
			   INNER JOIN Skills.SkillLevel AS SL ON i.SkillLevelId = SL.SkillLevelId
			   INNER JOIN dbo.Person AS P ON P.PersonId = i.PersonId
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
	SELECT 3 AS ActivityTypeID, --Added
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.SkillsDescription,
												NEW_VALUES.PersonId,
												NEW_VALUES.Person,
												NEW_VALUES.SkillLevel,
												NEW_VALUES.YearsExperience,
												NEW_VALUES.LastUsed
					    FROM NEW_VALUES
			           WHERE (NEW_VALUES.SkillId = i.SkillId AND NEW_VALUES.PersonId = i.PersonId)
					  FOR XML AUTO, ROOT('PersonSkill'))),
			LogData = (SELECT   NEW_VALUES.SkillId,
								NEW_VALUES.PersonId,
								NEW_VALUES.SkillLevelId,
								NEW_VALUES.YearsExperience,
								NEW_VALUES.LastUsed
					    FROM NEW_VALUES
			           WHERE (NEW_VALUES.SkillId = i.SkillId AND NEW_VALUES.PersonId = i.PersonId)
					  FOR XML AUTO, ROOT('PersonSkill'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
			FULL JOIN deleted AS d ON i.SkillId = d.SkillId AND i.PersonId = d.PersonId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  WHERE d.SkillId IS NULL AND d.PersonId IS NULL

	-- End logging session
	--EXEC dbo.SessionLogUnprepare

END

