-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-12-23
-- Description:	
-- =============================================
CREATE TRIGGER tr_TE_logUpdateDelete
   ON  dbo.TimeEntries
   AFTER UPDATE, DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL;

	WITH NEW_VALUES AS
	(
		SELECT 1 AS Tag,
			   NULL AS Parent, 
			   ins.[TimeEntryId] AS 'TimeEntryId'
			  ,CONVERT(VARCHAR(8), ins.[EntryDate], 101) AS 'EntryDate'
			  ,CONVERT(VARCHAR(8), ins.[ModifiedDate], 101) AS 'ModifiedDate'
			  ,ins.[MilestonePersonId] AS 'MilestonePersonId'
			  ,CAST(ins.[ActualHours] as decimal(20,2)) AS 'ActualHours'
			  ,CAST(ins.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
			  ,ins.[TimeTypeId] AS 'TimeTypeId'
			  ,ins.[ModifiedBy] AS 'ModifiedBy'
			  ,ins.[Note] AS 'Note'
			  ,CASE ins.[IsReviewed] WHEN 0 THEN 'False' ELSE 'True' END AS 'IsReviewed' 
			  ,CONVERT(VARCHAR(8), ins.[MilestoneDate], 101) AS 'MilestoneDate'
			  ,modp.LastName + ', ' + modp.FirstName AS 'ModifiedByName'
			  ,objp.LastName + ', ' + objp.FirstName AS 'ObjectName'
			  ,objp.PersonId AS 'ObjectPersonId'
			  ,m.Description AS 'Description'
			  ,m.MilestoneId AS 'MilestoneId'
			  ,proj.[name] AS 'ProjectName'
			  ,proj.ProjectId AS 'ProjectId'
			  ,clnt.[name] AS 'ClientName'
			  ,clnt.[ClientId] AS 'ClientId'
	    FROM inserted AS ins
	    INNER JOIN dbo.Person AS modp ON modp.PersonId = ins.ModifiedBy
	    INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = ins.MilestonePersonId
	    INNER JOIN dbo.Person AS objp ON objp.PersonId = mp.PersonId
	    INNER JOIN dbo.Milestone AS m  ON m.MilestoneId = mp.MilestoneId
	    INNER JOIN dbo.Project AS proj ON proj.ProjectId = m.ProjectId
	    INNER JOIN dbo.Client AS clnt ON proj.ClientId = clnt.ClientId
	),

	OLD_VALUES AS
	(
		SELECT   1 AS Tag,
			   NULL AS Parent, 
			   del.[TimeEntryId] AS 'TimeEntryId'
			  ,CONVERT(VARCHAR(8), del.[EntryDate], 101) AS 'EntryDate'
			  ,CONVERT(VARCHAR(8), del.[ModifiedDate], 101) AS 'ModifiedDate'
			  ,del.[MilestonePersonId] AS 'MilestonePersonId'
			  ,CAST(del.[ActualHours] as decimal(20,2)) AS 'ActualHours'
			  ,CAST(del.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
			  ,del.[TimeTypeId] AS 'TimeTypeId'
			  ,del.[ModifiedBy] AS 'ModifiedBy'
			  ,del.[Note] AS 'Note'
			  ,CASE del.[IsReviewed] WHEN 0 THEN 'False' ELSE 'True' END AS 'IsReviewed' 
			  ,CONVERT(VARCHAR(8), del.[MilestoneDate], 101) AS 'MilestoneDate'
			  ,modp.LastName + ', ' + modp.FirstName AS 'ModifiedByName'
			  ,objp.LastName + ', ' + objp.FirstName AS 'ObjectName'
			  ,objp.PersonId AS 'ObjectPersonId'
			  ,m.Description AS 'Description'
			  ,m.MilestoneId AS 'MilestoneId'
			  ,proj.[name] AS 'ProjectName'
			  ,proj.ProjectId AS 'ProjectId'
			  ,clnt.[name] AS 'ClientName'
			  ,clnt.[ClientId] AS 'ClientId'
	    FROM deleted AS del
	    INNER JOIN dbo.Person AS modp ON modp.PersonId = del.ModifiedBy
	    INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = del.MilestonePersonId
	    INNER JOIN dbo.Person AS objp ON objp.PersonId = mp.PersonId
	    INNER JOIN dbo.Milestone AS m  ON m.MilestoneId = mp.MilestoneId
	    INNER JOIN dbo.Project AS proj ON proj.ProjectId = m.ProjectId
	    INNER JOIN dbo.Client AS clnt ON proj.ClientId = clnt.ClientId
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
	           WHEN d.TimeEntryId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.TimeEntryId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.TimeEntryId = OLD_VALUES.TimeEntryId
			           WHERE NEW_VALUES.TimeEntryId = ISNULL(i.TimeEntryId, d.TimeEntryId) OR OLD_VALUES.TimeEntryId = ISNULL(i.TimeEntryId, d.TimeEntryId)
					  FOR XML AUTO, ROOT('TimeEntry'))),
			LogData = (SELECT NEW_VALUES.Parent, 
								NEW_VALUES.TimeEntryId,
								NEW_VALUES.EntryDate,
								NEW_VALUES.ModifiedDate,
								NEW_VALUES.MilestonePersonId,
								NEW_VALUES.TimeTypeId,
								NEW_VALUES.ModifiedBy,
								NEW_VALUES.MilestoneDate,
								NEW_VALUES.ObjectPersonId,
								NEW_VALUES.MilestoneId,
								NEW_VALUES.ProjectId,
								NEW_VALUES.ClientId,
								OLD_VALUES.Parent, 
								OLD_VALUES.TimeEntryId,
								OLD_VALUES.EntryDate,
								OLD_VALUES.ModifiedDate,
								OLD_VALUES.MilestonePersonId,
								OLD_VALUES.TimeTypeId,
								OLD_VALUES.ModifiedBy,
								OLD_VALUES.MilestoneDate,
								OLD_VALUES.ObjectPersonId,
								OLD_VALUES.MilestoneId,
								OLD_VALUES.ProjectId,
								OLD_VALUES.ClientId
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.TimeEntryId = OLD_VALUES.TimeEntryId
			           WHERE NEW_VALUES.TimeEntryId = ISNULL(i.TimeEntryId, d.TimeEntryId) OR OLD_VALUES.TimeEntryId = ISNULL(i.TimeEntryId, d.TimeEntryId)
					  FOR XML AUTO, ROOT('TimeEntry'), TYPE)
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.TimeEntryId = d.TimeEntryId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 
	 -- End logging session
	 EXEC dbo.SessionLogUnprepare
END

