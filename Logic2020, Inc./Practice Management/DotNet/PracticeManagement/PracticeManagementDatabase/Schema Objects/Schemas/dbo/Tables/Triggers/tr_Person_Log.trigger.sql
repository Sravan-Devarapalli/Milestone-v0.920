-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-14-2008
-- Updated by:	Srinivas.M
-- Update date:	05-21-2012
-- Description:	Logs the changes in the dbo.Person table.
-- =============================================
CREATE TRIGGER [dbo].[tr_Person_Log]
ON [dbo].[Person]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	IF (SELECT COUNT(ISNULL(i.PersonID,d.PersonID)) 
		FROM inserted AS i	
		FULL JOIN deleted AS d ON i.PersonID = d.PersonID 
		WHERE ISNULL(i.IsStrawman, d.IsStrawman) = 0
		) > 0
	BEGIN 

		;WITH NEW_VALUES AS
		(
			SELECT i.PersonId,
					CONVERT(NVARCHAR(10), i.HireDate, 101) AS HireDate,
					CONVERT(NVARCHAR(10), i.TerminationDate, 101) AS TerminationDate,
					i.Alias ,
					p.Name AS DefaultPractice,
					i.FirstName,
					i.LastName,
					s.Name AS PersonStatusName,
					i.EmployeeNumber,
					i.SeniorityId,
					r.Name AS Seniority,
					CASE WHEN i.IsDefaultManager = 1 THEN 'Yes' ELSE 'NO' END AS [IsDefaultManager],
					i.ManagerId,
					mngr.LastName + ', ' + mngr.FirstName as [ManagerName],
					i.TelephoneNumber,
					i.DivisionId,
					PD.DivisionName,
					i.PayChexID,
					CASE WHEN i.IsOffshore = 1 THEN 'YES' ELSE 'NO' END AS [IsOffshore]
			FROM inserted AS i
					LEFT JOIN dbo.Practice AS p ON i.DefaultPractice = p.PracticeId
					INNER JOIN dbo.PersonStatus AS s ON i.PersonStatusId = s.PersonStatusId
					LEFT JOIN dbo.Seniority AS r ON i.SeniorityId = r.SeniorityId
					LEFT JOIN dbo.Person as mngr ON mngr.PersonId = i.ManagerId
					LEFT JOIN dbo.PersonDivision PD ON PD.DivisionId = i.DivisionId
			WHERE i.IsStrawman = 0 
		),

		OLD_VALUES AS
		(
			SELECT d.PersonId,
					CONVERT(NVARCHAR(10), d.HireDate, 101) AS HireDate,
					CONVERT(NVARCHAR(10), d.TerminationDate, 101) AS TerminationDate,
					d.Alias ,
					p.Name AS DefaultPractice,
					d.FirstName,
					d.LastName,
					s.Name AS PersonStatusName,
					d.EmployeeNumber,
					d.SeniorityId,
					r.Name AS Seniority,
					CASE WHEN d.IsDefaultManager = 1 THEN 'Yes' ELSE 'NO' END AS [IsDefaultManager],
					d.ManagerId,
					mngr.LastName + ', ' + mngr.FirstName as [ManagerName],
					d.TelephoneNumber,
					d.DivisionId,
					PD.DivisionName,
					d.PayChexID,
					CASE WHEN d.IsOffshore = 1 THEN 'YES' ELSE 'NO' END AS [IsOffshore]
			FROM deleted AS d
					LEFT JOIN dbo.Practice AS p ON d.DefaultPractice = p.PracticeId
					INNER JOIN dbo.PersonStatus AS s ON d.PersonStatusId = s.PersonStatusId
					LEFT JOIN dbo.Seniority AS r ON d.SeniorityId = r.SeniorityId
					LEFT JOIN dbo.Person as mngr ON mngr.PersonId = d.ManagerId
					LEFT JOIN dbo.PersonDivision PD ON PD.DivisionId = d.DivisionId
			WHERE d.IsStrawman = 0
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
					WHEN d.PersonID IS NULL THEN 3
					WHEN i.PersonID IS NULL THEN 5
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
				Data =  CONVERT(NVARCHAR(MAX),(SELECT *
							FROM NEW_VALUES
									FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
							WHERE NEW_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID) OR OLD_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID)
							FOR XML AUTO, ROOT('Person'))),
				LogData = (SELECT NEW_VALUES.PersonId,
									NEW_VALUES.HireDate,
									NEW_VALUES.TerminationDate,
									NEW_VALUES.EmployeeNumber,
									NEW_VALUES.IsDefaultManager,
									NEW_VALUES.TelephoneNumber,
									OLD_VALUES.PersonId,
									OLD_VALUES.HireDate,
									OLD_VALUES.TerminationDate,
									OLD_VALUES.EmployeeNumber,
									OLD_VALUES.IsDefaultManager,
									OLD_VALUES.TelephoneNumber
							FROM NEW_VALUES
									FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
							WHERE NEW_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID) OR OLD_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID)
							FOR XML AUTO, ROOT('Person'), TYPE),
				@CurrentPMTime
		FROM inserted AS i
				FULL JOIN deleted AS d ON i.PersonID = d.PersonID 
				INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		WHERE ISNULL(i.IsStrawman, d.IsStrawman) = 0 
			AND (
					ISNULL(i.HireDate,'') <> ISNULL(d.HireDate,'')
					OR ISNULL(i.TerminationDate,'') <> ISNULL(d.TerminationDate,'')
					OR ISNULL(i.Alias,'') <> ISNULL(d.Alias,'')
					OR ISNULL(i.DefaultPractice,'') <> ISNULL(d.DefaultPractice,'')
					OR ISNULL(i.FirstName,'') <> ISNULL(d.FirstName,'')
					OR ISNULL(i.LastName,'') <> ISNULL(d.LastName,'')
					OR ISNULL(i.PersonStatusId,'') <> ISNULL(d.PersonStatusId,'')
					OR ISNULL(i.EmployeeNumber,'') <> ISNULL(d.EmployeeNumber,'')
					OR ISNULL(i.SeniorityId,0) <> ISNULL(d.SeniorityId,0)
					OR ISNULL(i.IsDefaultManager,'') <> ISNULL(d.IsDefaultManager,'')
					OR ISNULL(i.ManagerId,0) <> ISNULL(d.ManagerId,0)
					OR ISNULL(i.TelephoneNumber,'') <> ISNULL(d.TelephoneNumber,'')
					OR ISNULL(i.DivisionId,0) <> ISNULL(d.DivisionId,0)
					OR ISNULL(i.PayChexID,'') <> ISNULL(d.PayChexID,'')
					OR ISNULL(i.IsOffshore,'') <> ISNULL(d.IsOffshore,'')
				)

	END

	IF (SELECT COUNT(ISNULL(i.PersonID,d.PersonID)) 
		FROM inserted AS i	
		FULL JOIN deleted AS d ON i.PersonID = d.PersonID 
		WHERE ISNULL(i.IsStrawman, d.IsStrawman) = 1
		) > 0
	BEGIN 

		;WITH NEW_VALUES AS
		(
			SELECT i.PersonId,
					CONVERT(NVARCHAR(10), i.HireDate, 101) AS [HireDate],
					CONVERT(NVARCHAR(10), i.TerminationDate, 101) AS [TerminationDate],
					i.FirstName AS [Skill],
					i.LastName AS [Role],
					s.Name AS PersonStatusName,
					i.EmployeeNumber
			FROM inserted AS i
					INNER JOIN dbo.PersonStatus AS s ON i.PersonStatusId = s.PersonStatusId
			WHERE i.IsStrawman = 1
		),
		OLD_VALUES AS
		(
			SELECT d.PersonId,
					CONVERT(NVARCHAR(10), d.HireDate, 101) AS [HireDate],
					CONVERT(NVARCHAR(10), d.TerminationDate, 101) AS [TerminationDate],
					d.FirstName AS [Skill],
					d.LastName AS [Role],
					s.Name AS PersonStatusName,
					d.EmployeeNumber
			FROM deleted AS d
					INNER JOIN dbo.PersonStatus AS s ON d.PersonStatusId = s.PersonStatusId
			WHERE d.IsStrawman = 1
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
					WHEN d.PersonID IS NULL THEN 3
					WHEN i.PersonID IS NULL THEN 5
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
				Data =  CONVERT(NVARCHAR(MAX),(SELECT *
							FROM NEW_VALUES
									FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
							WHERE NEW_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID) OR OLD_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID)
							FOR XML AUTO, ROOT('Strawman'))),
				LogData = (SELECT NEW_VALUES.PersonId,
									NEW_VALUES.HireDate,
									NEW_VALUES.TerminationDate,
									NEW_VALUES.EmployeeNumber,
									OLD_VALUES.PersonId,
									OLD_VALUES.HireDate,
									OLD_VALUES.TerminationDate,
									OLD_VALUES.EmployeeNumber
							FROM NEW_VALUES
									FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
							WHERE NEW_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID) OR OLD_VALUES.PersonID = ISNULL(i.PersonId, d.PersonID)
							FOR XML AUTO, ROOT('Strawman'), TYPE),
				@CurrentPMTime
 		FROM inserted AS i
				FULL JOIN deleted AS d ON i.PersonID = d.PersonID 
				INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		WHERE ISNULL(i.IsStrawman, d.IsStrawman) = 1 
		AND (
				ISNULL(i.HireDate,'') <> ISNULL(d.HireDate,'')
				OR ISNULL(i.TerminationDate,'') <> ISNULL(d.TerminationDate,'')
				OR ISNULL(i.FirstName,'') <> ISNULL(d.FirstName,'')
				OR ISNULL(i.LastName,'') <> ISNULL(d.LastName,'')
				OR ISNULL(i.PersonStatusId,'') <> ISNULL(d.PersonStatusId,'')
				OR ISNULL(i.EmployeeNumber,'') <> ISNULL(d.EmployeeNumber,'')
			)
			
	END

		-- End logging session
	EXEC dbo.SessionLogUnprepare
END



