-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-12-23
-- Description:	Adds activity log record on TE insert
-- =============================================
CREATE TRIGGER tr_TE_logInsert 
   ON  dbo.TimeEntries
   AFTER INSERT
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

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
	SELECT 3 AS ActivityTypeID /* insert only */,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	       Data =  CONVERT(NVARCHAR(MAX),(SELECT   1 AS Tag,
							   NULL AS Parent, 
							   NEW_VALUES.[TimeEntryId] AS 'NEW_VALUES!1!TimeEntryId'
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[EntryDate], 101) AS 'NEW_VALUES!1!EntryDate'
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[ModifiedDate], 101) AS 'NEW_VALUES!1!ModifiedDate'
							  ,NEW_VALUES.[MilestonePersonId] AS 'NEW_VALUES!1!MilestonePersonId'
							  ,CAST(NEW_VALUES.[ActualHours] as decimal(20,2)) AS 'NEW_VALUES!1!ActualHours'
							  ,CAST(NEW_VALUES.[ForecastedHours] as decimal(20,2)) AS 'NEW_VALUES!1!ForecastedHours'
							  ,NEW_VALUES.[TimeTypeId] AS 'NEW_VALUES!1!TimeTypeId'
							  ,NEW_VALUES.[ModifiedBy] AS 'NEW_VALUES!1!ModifiedBy'
							  ,NEW_VALUES.[Note] AS 'NEW_VALUES!1!Note'
							  ,CASE NEW_VALUES.[IsReviewed] WHEN 0 THEN 'False' ELSE 'True' END AS 'NEW_VALUES!1!IsReviewed' 
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[MilestoneDate], 101) AS 'NEW_VALUES!1!MilestoneDate'
							  ,modp.LastName + ', ' + modp.FirstName AS 'NEW_VALUES!1!ModifiedByName'
							  ,objp.LastName + ', ' + objp.FirstName AS 'NEW_VALUES!1!ObjectName'
							  ,objp.PersonId AS 'NEW_VALUES!1!ObjectPersonId'
							  ,m.Description AS 'NEW_VALUES!1!Description'
							  ,m.MilestoneId AS 'NEW_VALUES!1!MilestoneId'
							  ,proj.[name] AS 'NEW_VALUES!1!ProjectName'
							  ,proj.ProjectId AS 'NEW_VALUES!1!ProjectId'
							  ,clnt.[name] AS 'NEW_VALUES!1!ClientName'
							  ,clnt.[ClientId] AS 'NEW_VALUES!1!ClientId'
					    FROM inserted AS NEW_VALUES
					    INNER JOIN dbo.Person AS modp ON modp.PersonId = NEW_VALUES.ModifiedBy
					    INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = NEW_VALUES.MilestonePersonId
					    INNER JOIN dbo.Person AS objp ON objp.PersonId = mp.PersonId
					    INNER JOIN dbo.Milestone AS m  ON m.MilestoneId = mp.MilestoneId
					    INNER JOIN dbo.Project AS proj ON proj.ProjectId = m.ProjectId
					    INNER JOIN dbo.Client AS clnt ON proj.ClientId = clnt.ClientId
			           WHERE NEW_VALUES.TimeEntryId = i.TimeEntryId
					  FOR XML EXPLICIT, ROOT('TimeEntry'))),
					  LogData = (SELECT   1 AS Tag,
							   NULL AS Parent, 
							   NEW_VALUES.[TimeEntryId] AS 'NEW_VALUES!1!TimeEntryId'
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[EntryDate], 101) AS 'NEW_VALUES!1!EntryDate'
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[ModifiedDate], 101) AS 'NEW_VALUES!1!ModifiedDate'
							  ,NEW_VALUES.[MilestonePersonId] AS 'NEW_VALUES!1!MilestonePersonId'
							  ,NEW_VALUES.[TimeTypeId] AS 'NEW_VALUES!1!TimeTypeId'
							  ,NEW_VALUES.[Note] AS 'NEW_VALUES!1!Note'
							  ,CONVERT(VARCHAR(8), NEW_VALUES.[MilestoneDate], 101) AS 'NEW_VALUES!1!MilestoneDate'
							  ,objp.LastName + ', ' + objp.FirstName AS 'NEW_VALUES!1!ObjectName'
							  ,m.MilestoneId AS 'NEW_VALUES!1!MilestoneId'
							  ,proj.ProjectId AS 'NEW_VALUES!1!ProjectId'
							  ,clnt.[ClientId] AS 'NEW_VALUES!1!ClientId'
					    FROM inserted AS NEW_VALUES
					    INNER JOIN dbo.Person AS modp ON modp.PersonId = NEW_VALUES.ModifiedBy
					    INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = NEW_VALUES.MilestonePersonId
					    INNER JOIN dbo.Person AS objp ON objp.PersonId = mp.PersonId
					    INNER JOIN dbo.Milestone AS m  ON m.MilestoneId = mp.MilestoneId
					    INNER JOIN dbo.Project AS proj ON proj.ProjectId = m.ProjectId
					    INNER JOIN dbo.Client AS clnt ON proj.ClientId = clnt.ClientId
			           WHERE NEW_VALUES.TimeEntryId = i.TimeEntryId
					  FOR XML EXPLICIT, ROOT('TimeEntry'), TYPE)
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	 -- End logging session
	 EXEC dbo.SessionLogUnprepare
END

