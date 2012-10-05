CREATE TRIGGER [Skills].[tr_PersonProfile_log]
ON [Skills].[PersonProfile]
FOR INSERT,UPDATE,DELETE
AS 
BEGIN
    SET NOCOUNT ON

	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.Id AS ProfileId,
				i.PersonId,
				P.LastName + ',' + P.FirstName AS Person,
				i.ProfileName,
				i.ProfileUrl,
				M.LastName + ',' + M.FirstName AS [ModifiedBy],
				i.IsDefault
		  FROM inserted AS i
			   INNER JOIN dbo.Person AS P ON P.PersonId = i.PersonId
			   INNER JOIN dbo.Person AS M ON M.PersonId = i.ModifiedBy
	),

	OLD_VALUES AS
	(
		SELECT d.Id AS ProfileId,
				d.PersonId,
				P.LastName + ',' + P.FirstName AS Person,
				d.ProfileName,
				d.ProfileUrl,
				M.LastName + ',' + M.FirstName AS [ModifiedBy],
				d.IsDefault
		  FROM deleted AS d
			   INNER JOIN dbo.Person AS P ON P.PersonId = d.PersonId
			   INNER JOIN dbo.Person AS M ON M.PersonId = d.ModifiedBy
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
	           WHEN i.Id IS NULL THEN 5 --Deleted
			   WHEN d.Id IS NULL THEN 3 --Inserted
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
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProfileId,
												NEW_VALUES.PersonId,
												NEW_VALUES.Person,
												NEW_VALUES.ProfileName,
												NEW_VALUES.ProfileUrl,
												CASE WHEN NEW_VALUES.IsDefault = 1 THEN 'YES' ELSE 'NO' END AS IsDefault,
												NEW_VALUES.[ModifiedBy],
												OLD_VALUES.ProfileId,
												OLD_VALUES.PersonId,
												OLD_VALUES.Person,
												OLD_VALUES.ProfileName,
												OLD_VALUES.ProfileUrl,
												CASE WHEN OLD_VALUES.IsDefault = 1 THEN 'YES' ELSE 'NO' END AS IsDefault,
												OLD_VALUES.[ModifiedBy]
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProfileId = OLD_VALUES.ProfileId
			           WHERE (NEW_VALUES.ProfileId = ISNULL(i.Id, d.Id)) OR (OLD_VALUES.ProfileId = ISNULL(d.Id, i.Id))
					  FOR XML AUTO, ROOT('PersonSkill'))),
			LogData = (SELECT NEW_VALUES.ProfileId,
								NEW_VALUES.PersonId,
								NEW_VALUES.ProfileName,
								NEW_VALUES.ProfileUrl,
								CASE WHEN NEW_VALUES.IsDefault = 1 THEN 'YES' ELSE 'NO' END AS IsDefault,
								OLD_VALUES.ProfileId,
								OLD_VALUES.PersonId,
								OLD_VALUES.ProfileName,
								OLD_VALUES.ProfileUrl,
								CASE WHEN OLD_VALUES.IsDefault = 1 THEN 'YES' ELSE 'NO' END AS IsDefault
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProfileId = OLD_VALUES.ProfileId
			           WHERE (NEW_VALUES.ProfileId = ISNULL(i.Id, d.Id)) OR (OLD_VALUES.ProfileId = ISNULL(d.Id, i.Id))
					  FOR XML AUTO, ROOT('PersonSkill'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.Id = d.Id
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.Id IS NULL-- Deleted record
			OR d.Id IS NULL -- INSERTED record
			--Updated Record
			OR i.IsDefault <> d.IsDefault
			OR i.ProfileName <> d.ProfileName
			OR i.ProfileUrl <> d.ProfileUrl

	-- End logging session
	--EXEC dbo.SessionLogUnprepare

END

