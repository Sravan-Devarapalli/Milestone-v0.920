CREATE TRIGGER [tr_Title_Log]
	ON [dbo].[Title]
	AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
  -- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.TitleId,
				i.Title,
				TT.TitleTypeId,
				TT.TitleType,
				i.SortOrder,
				i.PTOAccrual,
				i.MinimumSalary,
				i.MaximumSalary
		  FROM inserted AS i
		  INNER JOIN dbo.TitleType TT ON TT.TitleTypeId = i.TitleTypeId
	),

	OLD_VALUES AS
	(
		SELECT	d.TitleId,
				d.Title,
				TT.TitleTypeId,
				TT.TitleType,
				d.SortOrder,
				d.PTOAccrual,
				d.MinimumSalary,
				d.MaximumSalary
		  FROM deleted AS d
		  INNER JOIN dbo.TitleType TT ON TT.TitleTypeId = d.TitleTypeId
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
	           WHEN d.TitleId IS NULL THEN 3
	           WHEN i.TitleId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.TitleId = OLD_VALUES.TitleId
			           WHERE NEW_VALUES.TitleId = ISNULL(i.TitleId, d.TitleId) OR OLD_VALUES.TitleId = ISNULL(i.TitleId, d.TitleId)
					  FOR XML AUTO, ROOT('Title'))),
		LogData = (SELECT 
						NEW_VALUES.TitleId 
						,NEW_VALUES.Title
						,NEW_VALUES.TitleTypeId
						,NEW_VALUES.TitleType
						,NEW_VALUES.PTOAccrual 
						,NEW_VALUES.MinimumSalary
						,NEW_VALUES.MaximumSalary
						,OLD_VALUES.TitleId 
						,OLD_VALUES.Title
						,OLD_VALUES.TitleTypeId
						,OLD_VALUES.TitleType
						,OLD_VALUES.PTOAccrual 
						,OLD_VALUES.MinimumSalary
						,OLD_VALUES.MaximumSalary
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.TitleId = OLD_VALUES.TitleId
			           WHERE NEW_VALUES.TitleId = ISNULL(i.TitleId, d.TitleId) OR OLD_VALUES.TitleId = ISNULL(i.TitleId, d.TitleId)
					  FOR XML AUTO, ROOT('Title'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.TitleId = d.TitleId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

