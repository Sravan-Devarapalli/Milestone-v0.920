CREATE TRIGGER [Skills].[tr_PersonIndustry_LogInsert]
    ON [Skills].[PersonIndustry]
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
				Ind.Description 'IndustryDescription',
				P.LastName + ', ' + P.FirstName Person
		  FROM inserted AS i
			   INNER JOIN Skills.Industry AS Ind ON Ind.IndustryId = i.IndustryId
			   INNER JOIN dbo.Person P ON P.PersonId = i.PersonId
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
	SELECT 3 AS ActivityTypeID,
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
													NEW_VALUES.YearsExperience
					    FROM NEW_VALUES
			           WHERE (NEW_VALUES.IndustryId = i.IndustryId AND NEW_VALUES.PersonId = i.PersonId)
					  FOR XML AUTO, ROOT('PersonIndustry'))),
			LogData = (SELECT   NEW_VALUES.IndustryId,
								NEW_VALUES.IndustryDescription,
								NEW_VALUES.PersonId,
								NEW_VALUES.Person,
								NEW_VALUES.YearsExperience
					    FROM NEW_VALUES
			           WHERE (NEW_VALUES.IndustryId = i.IndustryId AND NEW_VALUES.PersonId = i.PersonId)
					  FOR XML AUTO, ROOT('PersonIndustry'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	-- End logging session
	--EXEC dbo.SessionLogUnprepare

    END

