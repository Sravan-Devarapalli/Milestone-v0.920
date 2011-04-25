
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-27-2008
-- Updated by:	
-- Update date:	
-- Description:	Logs the updating and deletring from the dbo.Project table.
-- =============================================
CREATE TRIGGER dbo.tr_Project_LogUpdateDelete
ON dbo.[Project]
AFTER UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	;WITH NEW_VALUES AS
	(
		SELECT i.ProjectId,
		       i.Name,
		       c.Name AS ClientName,
		       i.Discount,
		       i.Terms,
		       p.PracticeManagerId,
		       pm.LastName + ', ' + pm.FirstName AS PracticeManagerFullName,
		       p.Name AS PracticeName,
		       CONVERT(NVARCHAR(10), i.StartDate, 101) AS StartDate,
		       CONVERT(NVARCHAR(10), i.EndDate, 101) AS EndDate,
		       s.Name AS ProjectStatusName,
		       i.ProjectNumber,
		       i.BuyerName
		  FROM inserted AS i
		       INNER JOIN dbo.Client AS c ON i.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON i.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON i.ProjectStatusId = s.ProjectStatusId
	),

	OLD_VALUES AS
	(
		SELECT d.ProjectId,
		       d.Name,
		       c.Name AS ClientName,
		       d.Discount,
		       d.Terms,
		       p.PracticeManagerId,
		       pm.LastName + ', ' + pm.FirstName AS PracticeManagerFullName,
		       p.Name AS PracticeName,
		       CONVERT(NVARCHAR(10), d.StartDate, 101) AS StartDate,
		       CONVERT(NVARCHAR(10), d.EndDate, 101) AS EndDate,
		       s.Name AS ProjectStatusName,
		       d.ProjectNumber,
		       d.BuyerName
		  FROM deleted AS d
		       INNER JOIN dbo.Client AS c ON d.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON d.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON d.ProjectStatusId = s.ProjectStatusId
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
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			LogData = (SELECT 
							NEW_VALUES.ProjectId,
							NEW_VALUES.Terms,
							NEW_VALUES.PracticeManagerId,
							NEW_VALUES.StartDate,
							NEW_VALUES.EndDate,
							NEW_VALUES.ProjectNumber,
							OLD_VALUES.ProjectId,
							OLD_VALUES.Terms,
							OLD_VALUES.PracticeManagerId,
							OLD_VALUES.StartDate,
							OLD_VALUES.EndDate,
							OLD_VALUES.ProjectNumber
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'), TYPE)
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL -- deleted record
	    -- Detect changes
	    OR ISNULL(i.ClientId, 0) <> ISNULL(d.ClientId, 0)
	    OR ISNULL(i.Discount, 0) <> ISNULL(d.Discount, 0)
	    OR ISNULL(i.Terms, 0) <> ISNULL(d.Terms, 0)
	    OR i.Name <> d.Name
	    OR ISNULL(i.PracticeId, 0) <> ISNULL(d.PracticeId, 0)
	    OR ISNULL(i.ProjectStatusId, 0) <> ISNULL(d.ProjectStatusId, 0)
	    OR ISNULL(i.ProjectNumber, '') <> ISNULL(d.ProjectNumber, '')
	    OR ISNULL(i.BuyerName, '') <> ISNULL(d.BuyerName, '')
	
	-- End logging session
	 EXEC dbo.SessionLogUnprepare
END

