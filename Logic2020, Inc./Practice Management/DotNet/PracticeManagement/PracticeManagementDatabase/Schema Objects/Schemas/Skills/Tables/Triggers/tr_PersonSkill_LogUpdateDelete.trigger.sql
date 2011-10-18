CREATE TRIGGER [Skills].[tr_PersonSkill_LogUpdateDelete]
ON [Skills].[PersonSkill]
FOR DELETE, UPDATE 
AS 
BEGIN
    SET NOCOUNT ON

	-- Be sure to prepare Session log while update or delete rows in PersonSkill.
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
	),

	OLD_VALUES AS
	(
		SELECT d.*,
				S.Description 'SkillsDescription',
				SL.Description 'SkillLevel',
				P.LastName + ', ' + P.FirstName [Person]
		  FROM deleted AS d
			   INNER JOIN Skills.Skill AS S ON S.SkillId = d.SkillId
			   INNER JOIN Skills.SkillLevel AS SL ON d.SkillLevelId = SL.SkillLevelId
			   INNER JOIN dbo.Person AS P ON P.PersonId = d.PersonId
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
	           WHEN i.SkillId IS NULL AND i.PersonId IS NULL THEN 5 --Deleted
	           ELSE 4 --Changed
	       END AS ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.SkillsDescription,
												NEW_VALUES.Person,
												NEW_VALUES.SkillLevel,
												NEW_VALUES.YearsExperience,
												NEW_VALUES.LastUsed,
												OLD_VALUES.SkillsDescription,
												OLD_VALUES.Person,
												OLD_VALUES.SkillLevel,
												OLD_VALUES.YearsExperience,
												OLD_VALUES.LastUsed
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.SkillId = OLD_VALUES.SkillId AND NEW_VALUES.PersonId = OLD_VALUES.PersonId
			           WHERE (NEW_VALUES.SkillId = ISNULL(i.SkillId, d.SkillId) AND NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId))
							OR (OLD_VALUES.SkillId = ISNULL(d.SkillId, i.SkillId) AND  OLD_VALUES.PersonId = ISNULL(d.PersonId, i.PersonId))
					  FOR XML AUTO, ROOT('PersonSkill'))),
			LogData = (SELECT   NEW_VALUES.SkillId,
								NEW_VALUES.PersonId,
								NEW_VALUES.SkillLevelId,
								NEW_VALUES.YearsExperience,
								NEW_VALUES.LastUsed,
								OLD_VALUES.SkillId,
								OLD_VALUES.PersonId,
								OLD_VALUES.SkillLevelId,
								OLD_VALUES.YearsExperience,
								OLD_VALUES.LastUsed
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.SkillId = OLD_VALUES.SkillId AND NEW_VALUES.PersonId = OLD_VALUES.PersonId
			           WHERE (NEW_VALUES.SkillId = ISNULL(i.SkillId, d.SkillId) AND NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId))
							OR (OLD_VALUES.SkillId = ISNULL(d.SkillId, i.SkillId) AND  OLD_VALUES.PersonId = ISNULL(d.PersonId, i.PersonId))
					  FOR XML AUTO, ROOT('PersonSkill'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.SkillId = d.SkillId AND i.PersonId = d.PersonId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE (i.SkillId IS NULL AND i.PersonId IS NULL)-- Deleted record
			OR i.SkillLevelId <> d.SkillLevelId
			OR i.YearsExperience <> d.YearsExperience
			OR i.LastUsed <> d.LastUsed

	-- End logging session
	--EXEC dbo.SessionLogUnprepare

END

