﻿CREATE TRIGGER [dbo].[tr_Project_LogUpdateDelete]
ON [dbo].[Project]
AFTER UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

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
				,i.GroupId
				,PG.Name AS 'ProjectGroup'
				,i.Description
				,i.DirectorId
				,CASE WHEN i.DirectorId IS NOT NULL THEN Dir.LastName + ', ' + Dir.FirstName 
				      ELSE '' 
				      END AS 'ClientDirector'
				,CASE WHEN i.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable',
				i.ProjectOwnerId,
				ProjOwner.LastName + ', ' + ProjOwner.FirstName AS [ProjectOwner]
				, i.SowBudget
				,i.BusinessTypeId
				,bt.Name AS [BusinessType]
				,i.PricingListId
				,pt.Name AS [PricingList]
				,i.SeniorManagerId
				,CASE WHEN i.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE SenManager.LastName + ', ' + SenManager.FirstName END AS SeniorManager
				,i.[ReviewerId]
				,Rev.LastName + ', ' + Rev.FirstName AS [Reviewer]
				,i.PONumber
				,i.POAmount
				,i.SalesPersonId
				,CASE WHEN i.SalesPersonId IS NOT NULL THEN salesPerson.LastName + ', ' + salesPerson.FirstName 
				      ELSE '' 
				      END AS 'SalesPerson'
		  FROM inserted AS i
		       INNER JOIN dbo.Client AS c ON i.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON i.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON i.ProjectStatusId = s.ProjectStatusId
			   LEFT JOIN dbo.Person AS ProjOwner ON ProjOwner.PersonId = i.ProjectOwnerId
			   INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = i.GroupId
			   LEFT  JOIN dbo.Person AS Dir ON Dir.PersonId = i.DirectorId
			   LEFT JOIN dbo.BusinessType bt ON bt.BusinessTypeId = i.BusinessTypeId
			   LEFT JOIN dbo.PricingList pt ON pt.PricingListId = i.PricingListId
			   LEFT JOIN dbo.Person AS SenManager ON SenManager.PersonId = i.SeniorManagerId
			   LEFT JOIN dbo.Person AS Rev ON Rev.PersonId = i.[ReviewerId]
			   LEFT JOIN dbo.Person AS salesPerson ON salesPerson.PersonId = i.[SalesPersonId]
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
				,d.GroupId
				,PG.Name AS 'ProjectGroup'
				,d.Description
				,d.DirectorId
				,CASE WHEN d.DirectorId IS NOT NULL THEN Dir.LastName + ', ' + Dir.FirstName 
				      ELSE '' 
				      END AS 'ClientDirector'
				,CASE WHEN d.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable',
						d.ProjectOwnerId,
				ProjOwner.LastName + ', ' + ProjOwner.FirstName AS [ProjectOwner]
				,d.SowBudget
				,d.BusinessTypeId
				,bt.Name AS [BusinessType]
				,d.PricingListId
				,pt.Name AS [PricingList]
				,d.SeniorManagerId
				,CASE WHEN d.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE SenManager.LastName + ', ' + SenManager.FirstName END AS SeniorManager
				,d.[ReviewerId]
				,Rev.LastName + ', ' + Rev.FirstName AS [Reviewer]
				,d.PONumber
				,d.POAmount
				,d.SalesPersonId
				,CASE WHEN d.SalesPersonId IS NOT NULL THEN salesPerson.LastName + ', ' + salesPerson.FirstName 
				      ELSE '' 
				      END AS 'SalesPerson'
		  FROM deleted AS d
		       INNER JOIN dbo.Client AS c ON d.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON d.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON d.ProjectStatusId = s.ProjectStatusId
			   LEFT JOIN dbo.Person AS ProjOwner ON ProjOwner.PersonId = d.ProjectOwnerId
			   INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = d.GroupId
			   LEFT JOIN dbo.Person AS Dir ON Dir.PersonId = d.DirectorId
			   LEFT JOIN dbo.BusinessType bt ON bt.BusinessTypeId = d.BusinessTypeId
			   LEFT JOIN dbo.PricingList pt ON pt.PricingListId = d.PricingListId
			   LEFT JOIN dbo.Person AS SenManager ON SenManager.PersonId = d.SeniorManagerId
			   LEFT JOIN dbo.Person AS Rev ON Rev.PersonId = d.[ReviewerId]
			   LEFT JOIN dbo.Person AS salesPerson ON salesPerson.PersonId = d.[SalesPersonId]
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
							NEW_VALUES.Description,
							OLD_VALUES.ProjectId,
							OLD_VALUES.Terms,
							OLD_VALUES.PracticeManagerId,
							OLD_VALUES.StartDate,
							OLD_VALUES.EndDate,
							OLD_VALUES.ProjectNumber,
							OLD_VALUES.Description
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL -- deleted record
	    -- Detect changes
	    OR ISNULL(i.ClientId, 0) <> ISNULL(d.ClientId, 0)
	    OR ISNULL(i.DirectorId, 0) <> ISNULL(d.DirectorId, 0)
	    OR ISNULL(i.GroupId, 0) <> ISNULL(d.GroupId,0)
	    OR ISNULL(i.Discount, 0) <> ISNULL(d.Discount, 0)
	    OR ISNULL(i.Terms, 0) <> ISNULL(d.Terms, 0)
	    OR i.Name <> d.Name
	    OR ISNULL(i.PracticeId, 0) <> ISNULL(d.PracticeId, 0)
	    OR ISNULL(i.ProjectStatusId, 0) <> ISNULL(d.ProjectStatusId, 0)
	    OR ISNULL(i.ProjectNumber, '') <> ISNULL(d.ProjectNumber, '')
	    OR ISNULL(i.BuyerName, '') <> ISNULL(d.BuyerName, '')
	    OR ISNULL(i.StartDate, '2029-10-31') <> ISNULL(d.StartDate, '2029-10-31')
	    OR ISNULL(i.EndDate, '2029-10-31') <> ISNULL(d.EndDate, '2029-10-31')
	    OR i.IsChargeable <> d.IsChargeable
		OR i.ProjectOwnerId <> d.ProjectOwnerId
		OR ISNULL(i.[ReviewerId], 0) <> ISNULL(d.[ReviewerId], 0)
		OR ISNULL(i.SeniorManagerId, 0) <> ISNULL(d.SeniorManagerId, 0)
	    OR ISNULL(i.Description,'') <> ISNULL(d.Description, '')
	    OR ISNULL(i.SowBudget, 0) <> ISNULL(d.SowBudget, 0)
		OR ISNULL(i.BusinessTypeId, 0) <> ISNULL(d.BusinessTypeId, 0)
		OR ISNULL(i.PricingListId, 0) <> ISNULL(d.PricingListId, 0)
		OR ISNULL(i.[PONumber], '') <> ISNULL(d.[PONumber], '')
		OR ISNULL(i.[POAmount], 0) <> ISNULL(d.[POAmount], 0)
		OR ISNULL(i.[SalesPersonId], 0) <> ISNULL(d.[SalesPersonId], 0)
	-------------------
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
				,i.GroupId
				,PG.Name AS 'ProjectGroup'
				,i.Description
				,i.DirectorId
				,CASE WHEN i.DirectorId IS NOT NULL THEN Dir.LastName + ', ' + Dir.FirstName 
				      ELSE '' 
				      END AS 'ClientDirector'
				,CASE WHEN i.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable',
				i.ProjectOwnerId,
				ProjOwner.LastName + ', ' + ProjOwner.FirstName AS [ProjectOwner]
				, i.SowBudget
				,i.BusinessTypeId
				,bt.Name AS [BusinessType]
				,i.PricingListId
				,pt.Name AS [PricingList]
				,i.SeniorManagerId
				,CASE WHEN i.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE SenManager.LastName + ', ' + SenManager.FirstName END AS SeniorManager
				,i.[ReviewerId]
				,Rev.LastName + ', ' + Rev.FirstName AS [Reviewer]
				,i.PONumber
				,i.POAmount
				,i.SalesPersonId
				,CASE WHEN i.SalesPersonId IS NOT NULL THEN salesPerson.LastName + ', ' + salesPerson.FirstName 
				      ELSE '' 
				      END AS 'SalesPerson'
		  FROM inserted AS i
		       INNER JOIN dbo.Client AS c ON i.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON i.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON i.ProjectStatusId = s.ProjectStatusId
			   LEFT JOIN dbo.Person AS ProjOwner ON ProjOwner.PersonId = i.ProjectOwnerId
			   INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = i.GroupId
			   LEFT  JOIN dbo.Person AS Dir ON Dir.PersonId = i.DirectorId
			   LEFT JOIN dbo.BusinessType bt ON bt.BusinessTypeId = i.BusinessTypeId
			   LEFT JOIN dbo.PricingList pt ON pt.PricingListId = i.PricingListId
			   LEFT JOIN dbo.Person AS SenManager ON SenManager.PersonId = i.SeniorManagerId
			   LEFT JOIN dbo.Person AS Rev ON Rev.PersonId = i.[ReviewerId]
			   LEFT JOIN dbo.Person AS salesPerson ON salesPerson.PersonId = i.[SalesPersonId]
	),

	OLD_VALUES AS
	(
		SELECT d.ProjectId,
		       d.Name,
			   i.Name as NewName,
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
				,d.GroupId
				,PG.Name AS 'ProjectGroup'
				,d.Description
				,d.DirectorId
				,CASE WHEN d.DirectorId IS NOT NULL THEN Dir.LastName + ', ' + Dir.FirstName 
				      ELSE '' 
				      END AS 'ClientDirector'
				,CASE WHEN d.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable',
						d.ProjectOwnerId,
				ProjOwner.LastName + ', ' + ProjOwner.FirstName AS [ProjectOwner]
				,d.SowBudget
				,d.BusinessTypeId
				,bt.Name AS [BusinessType]
				,d.PricingListId
				,pt.Name AS [PricingList]
				,d.SeniorManagerId
				,CASE WHEN d.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE SenManager.LastName + ', ' + SenManager.FirstName END AS SeniorManager
				,d.[ReviewerId]
				,Rev.LastName + ', ' + Rev.FirstName AS [Reviewer]
				,d.PONumber
				,d.POAmount
				,d.SalesPersonId
				,CASE WHEN d.SalesPersonId IS NOT NULL THEN salesPerson.LastName + ', ' + salesPerson.FirstName 
				      ELSE '' 
				      END AS 'SalesPerson'
		  FROM deleted AS d
		       INNER JOIN dbo.Client AS c ON d.ClientId = c.ClientId
		       INNER JOIN dbo.Practice AS p ON d.PracticeId = p.PracticeId
		       INNER JOIN dbo.Person AS pm ON p.PracticeManagerId = pm.PersonId
		       INNER JOIN dbo.ProjectStatus AS s ON d.ProjectStatusId = s.ProjectStatusId
			   LEFT JOIN dbo.Person AS ProjOwner ON ProjOwner.PersonId = d.ProjectOwnerId
			   INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = d.GroupId
			   LEFT JOIN dbo.Person AS Dir ON Dir.PersonId = d.DirectorId
			   LEFT JOIN dbo.BusinessType bt ON bt.BusinessTypeId = d.BusinessTypeId
			   LEFT JOIN dbo.PricingList pt ON pt.PricingListId = d.PricingListId
			   LEFT JOIN dbo.Person AS SenManager ON SenManager.PersonId = d.SeniorManagerId
			   LEFT JOIN dbo.Person AS Rev ON Rev.PersonId = d.[ReviewerId]
			   LEFT JOIN dbo.Person AS salesPerson ON salesPerson.PersonId = d.[SalesPersonId]
			   Left join inserted i on i.ProjectId = d.ProjectId
	)

	-- Log an activity
	INSERT INTO dbo.UserActivityLogRecordPerChange
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
	             LogDate)
	SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,OLD_VALUES.ProjectId,OLD_VALUES.Name
		               FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR i.Name <> d.Name
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.ClientName,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.ClientName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.ClientId, 0) <> ISNULL(d.ClientId, 0)
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.DirectorId,NEW_VALUES.ClientDirector,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.DirectorId,OLD_VALUES.ClientDirector
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.DirectorId, 0) <> ISNULL(d.DirectorId, 0)
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.GroupId,NEW_VALUES.ProjectGroup,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.GroupId,OLD_VALUES.ProjectGroup
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.GroupId, 0) <> ISNULL(d.GroupId, 0)   
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.Discount,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.Discount
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.Discount, 0) <> ISNULL(d.Discount, 0)   
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.Terms,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.Terms
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.Terms, 0) <> ISNULL(d.Terms, 0)   
	  UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.PracticeName,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.PracticeName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.PracticeId, 0) <> ISNULL(d.PracticeId, 0)   
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.PracticeManagerId,NEW_VALUES.PracticeManagerFullName,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.PracticeManagerId,OLD_VALUES.PracticeManagerFullName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.PracticeId, 0) <> ISNULL(d.PracticeId, 0)   
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.ProjectStatusName,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.ProjectStatusName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.ProjectStatusId, 0) <> ISNULL(d.ProjectStatusId, 0)   
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.ProjectNumber,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.ProjectNumber
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.ProjectNumber, '') <> ISNULL(d.ProjectNumber, '')   	
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.BuyerName,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.BuyerName
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.BuyerName, '') <> ISNULL(d.BuyerName, '')   	
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.StartDate,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.StartDate
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.StartDate, '2029-10-31') <> ISNULL(d.StartDate, '2029-10-31')
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.EndDate,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.EndDate
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.EndDate, '2029-10-31') <> ISNULL(d.EndDate, '2029-10-31')
	  UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.IsChargeable,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.IsChargeable
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR i.IsChargeable <> d.IsChargeable
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.ProjectOwnerId,NEW_VALUES.ProjectOwner,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.ProjectOwnerId,OLD_VALUES.ProjectOwner
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR i.ProjectOwnerId <> d.ProjectOwnerId
	UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.ReviewerId,NEW_VALUES.Reviewer,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.ReviewerId,OLD_VALUES.Reviewer
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.[ReviewerId], 0) <> ISNULL(d.[ReviewerId], 0)
		-- Detect changes
	UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.SeniorManagerId,NEW_VALUES.SeniorManager,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.SeniorManagerId,OLD_VALUES.SeniorManager
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.SeniorManagerId, 0) <> ISNULL(d.SeniorManagerId, 0)
	 	UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.Description,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.Description
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.Description,'') <> ISNULL(d.Description, '')
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.SowBudget,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.SowBudget
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.SowBudget, 0) <> ISNULL(d.SowBudget, 0)
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.BusinessTypeId,NEW_VALUES.BusinessType,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.BusinessTypeId,OLD_VALUES.BusinessType
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.BusinessTypeId, 0) <> ISNULL(d.BusinessTypeId, 0)
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.PricingListId,NEW_VALUES.PricingList,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.PricingListId,OLD_VALUES.PricingList
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.PricingListId, 0) <> ISNULL(d.PricingListId, 0)
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.POAmount,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.POAmount
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.[POAmount], 0) <> ISNULL(d.[POAmount], 0)
	  UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.PONumber,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.PONumber
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.[PONumber], '') <> ISNULL(d.[PONumber], '')
	 UNION ALL
	 SELECT CASE
	           WHEN d.ProjectId IS NULL THEN 3 -- Actually is redundant
	           WHEN i.ProjectId IS NULL THEN 5
	           ELSE 4
	       END AS ActivityTypeID,l.SessionID,l.SystemUser,l.Workstation,l.ApplicationName,l.UserLogin,l.PersonID,l.LastName,l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.ProjectId,NEW_VALUES.Name,NEW_VALUES.SalesPersonId,NEW_VALUES.SalesPerson,OLD_VALUES.ProjectId,OLD_VALUES.NewName as Name,OLD_VALUES.SalesPersonId,OLD_VALUES.SalesPerson
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.ProjectId = OLD_VALUES.ProjectId
			           WHERE NEW_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId) OR OLD_VALUES.ProjectId = ISNULL(i.ProjectId, d.ProjectId)
					  FOR XML AUTO, ROOT('Project'))),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.ProjectId = d.ProjectId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.ProjectId IS NULL OR ISNULL(i.[SalesPersonId], 0) <> ISNULL(d.[SalesPersonId], 0)
		
	-- End logging session
	 EXEC dbo.SessionLogUnprepare
END

