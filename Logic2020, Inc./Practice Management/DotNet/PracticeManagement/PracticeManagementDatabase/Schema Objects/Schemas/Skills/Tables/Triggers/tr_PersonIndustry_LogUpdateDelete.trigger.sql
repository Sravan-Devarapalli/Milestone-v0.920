CREATE TRIGGER [Skills].[tr_PersonIndustry_LogUpdateDelete]
    ON [Skills].[PersonIndustry]
    FOR DELETE, UPDATE 
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
				Ind.Description 'IndustryDescription',
				P.LastName + ', ' + P.FirstName Person
		  FROM inserted AS i
			   INNER JOIN Skills.Industry AS Ind ON Ind.IndustryId = i.IndustryId
			   INNER JOIN dbo.Person P ON P.PersonId = i.PersonId
	),

	OLD_VALUES AS
	(
		SELECT d.*,
				Ind.Description 'IndustryDescription',
				P.LastName + ', ' + P.FirstName Person
		  FROM deleted AS d
			   INNER JOIN Skills.Industry AS Ind ON Ind.IndustryId = d.IndustryId
			   INNER JOIN dbo.Person P ON P.PersonId = d.PersonId
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
	           WHEN i.IndustryId IS NULL AND i.PersonId IS NULL THEN 5--Deleted
	           ELSE 4--Changed
	       END AS ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.IndustryDescription,
													NEW_VALUES.Person,
													NEW_VALUES.YearsExperience,
													OLD_VALUES.IndustryDescription,
													OLD_VALUES.Person,
													OLD_VALUES.YearsExperience
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.IndustryId = OLD_VALUES.IndustryId AND NEW_VALUES.PersonId = OLD_VALUES.PersonId
			           WHERE (NEW_VALUES.IndustryId = ISNULL(i.IndustryId, d.IndustryId) AND NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId))
							OR (OLD_VALUES.IndustryId = ISNULL(d.IndustryId, i.IndustryId) AND NEW_VALUES.PersonId = ISNULL(d.PersonId, i.PersonId))
					  FOR XML AUTO, ROOT('PersonIndustry'))),
			LogData = (SELECT   NEW_VALUES.IndustryId,
								NEW_VALUES.IndustryDescription,
								NEW_VALUES.PersonId,
								NEW_VALUES.Person,
								NEW_VALUES.YearsExperience,
								OLD_VALUES.IndustryId,
								OLD_VALUES.IndustryDescription,
								OLD_VALUES.PersonId,
								OLD_VALUES.Person,
								OLD_VALUES.YearsExperience
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.IndustryId = OLD_VALUES.IndustryId
			           WHERE (NEW_VALUES.IndustryId = ISNULL(i.IndustryId, d.IndustryId) AND NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId))
							OR (OLD_VALUES.IndustryId = ISNULL(d.IndustryId, i.IndustryId) AND NEW_VALUES.PersonId = ISNULL(d.PersonId, i.PersonId))
					  FOR XML AUTO, ROOT('PersonIndustry'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.IndustryId = d.IndustryId AND i.PersonId = d.PersonId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE (i.IndustryId IS NULL AND i.PersonId IS NULL) -- Deleted record
			OR (i.YearsExperience <> d.YearsExperience)

	-- End logging session
	--EXEC dbo.SessionLogUnprepare

    END

