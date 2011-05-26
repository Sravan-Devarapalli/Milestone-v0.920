-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-14-2008
-- Updated by:	
-- Update date:	
-- Description:	Logs the changes in the dbo.Person table.
-- =============================================
CREATE TRIGGER [dbo].[tr_Person_Log]
ON [dbo].[Person]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @GMT NVARCHAR(10) = (SELECT Value FROM Settings WHERE SettingsKey = 'TimeZone')
	DECLARE @CurrentPMTime DATETIME = (CASE WHEN CHARINDEX('-',@GMT) >0 THEN GETUTCDATE() - REPLACE(@GMT,'-','') ELSE 
											GETUTCDATE() + @GMT END)

	;WITH NEW_VALUES AS
	(
		SELECT i.PersonId,
		       i.PTODaysPerAnnum,
		       CONVERT(NVARCHAR(10), i.HireDate, 101) AS HireDate,
		       CONVERT(NVARCHAR(10), i.TerminationDate, 101) AS TerminationDate,
		       i.Alias,
		       p.Name AS DefaultPractice,
		       i.FirstName,
		       i.LastName,
		       s.Name AS PersonStatusName,
		       i.EmployeeNumber,
		       r.Name AS Seniority,
		       i.IsDefaultManager,
		       mngr.LastName + ', ' + mngr.FirstName as 'ManagerName',
			   i.TelephoneNumber
		  FROM inserted AS i
		       LEFT JOIN dbo.Practice AS p ON i.DefaultPractice = p.PracticeId
		       INNER JOIN dbo.PersonStatus AS s ON i.PersonStatusId = s.PersonStatusId
		       LEFT JOIN dbo.Seniority AS r ON i.SeniorityId = r.SeniorityId
		       INNER JOIN dbo.Person as mngr ON mngr.PersonId = i.ManagerId
	),

	OLD_VALUES AS
	(
		SELECT d.PersonId,
		       d.PTODaysPerAnnum,
		       CONVERT(NVARCHAR(10), d.HireDate, 101) AS HireDate,
		       CONVERT(NVARCHAR(10), d.TerminationDate, 101) AS TerminationDate,
		       d.Alias,
		       p.Name AS DefaultPractice,
		       d.FirstName,
		       d.LastName,
		       s.Name AS PersonStatusName,
		       d.EmployeeNumber,
		       r.Name AS Seniority,
		       d.IsDefaultManager,
		       mngr.LastName + ', ' + mngr.FirstName as 'ManagerName',
			   d.TelephoneNumber
		  FROM deleted AS d
		       LEFT JOIN dbo.Practice AS p ON d.DefaultPractice = p.PracticeId
		       INNER JOIN dbo.PersonStatus AS s ON d.PersonStatusId = s.PersonStatusId
		       LEFT JOIN dbo.Seniority AS r ON d.SeniorityId = r.SeniorityId
		       INNER JOIN dbo.Person as mngr ON mngr.PersonId = d.ManagerId
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
							  NEW_VALUES.PTODaysPerAnnum,
							  NEW_VALUES.HireDate,
							  NEW_VALUES.TerminationDate,
							  NEW_VALUES.EmployeeNumber,
							  NEW_VALUES.IsDefaultManager,
							  NEW_VALUES.TelephoneNumber,
							  OLD_VALUES.PersonId,
							  OLD_VALUES.PTODaysPerAnnum,
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

	  -- End logging session
	EXEC dbo.SessionLogUnprepare
END


GO



