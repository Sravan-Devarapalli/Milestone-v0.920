-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-25-2008
-- Updated By:	Srinivas.M
-- Updated Date: 2012-06-05
-- Description:	Logs the inserting into the dbo.Project table.
-- =============================================
CREATE TRIGGER [dbo].[tr_Project_LogInsert]
ON [dbo].[Project]
AFTER INSERT
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.ProjectId
				,i.ProjectNumber AS 'ProjectNumber'
				,i.Name AS 'Name'
				,i.ClientId
				,C.Name AS 'ClientName'
				,i.PracticeId
				,prac.Name AS 'PracticeArea'
				,i.ProjectStatusId
				,ps.Name AS 'ProjectStatusName'
				,i.Discount
				,i.BuyerName AS 'BuyerName'
				,i.GroupId
				,PG.Name AS 'ProjectGroup'
				,i.Description
				,i.DirectorId
				,CASE WHEN i.DirectorId IS NOT NULL THEN D.LastName + ', ' + D.FirstName 
				      ELSE '' 
				      END AS 'ClientDirector'
				,CASE WHEN i.IsChargeable = 1 THEN 'Yes'
						ELSE 'No' END AS 'IsChargeable',
				i.ProjectOwnerId,
				ProjOwner.LastName + ', ' + ProjOwner.FirstName AS [ProjectOwner]
				, i.SowBudget
		FROM inserted AS i
		INNER JOIN dbo.Client AS C ON C.ClientId = i.ClientId
		INNER JOIN dbo.Practice AS prac ON prac.PracticeId = i.PracticeId
		INNER JOIN dbo.ProjectStatus AS ps ON ps.ProjectStatusId = i.ProjectStatusId
		INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = i.GroupId
		LEFT JOIN dbo.Person AS ProjOwner ON ProjOwner.PersonId = i.ProjectOwnerId -- While Converting opportunity to Project ProjectOwnerId will not be there.So here Left join is used instead of INNER JOIN.
		LEFT JOIN dbo.Person AS D ON D.PersonId = i.DirectorId
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
	SELECT 3 AS ActivityTypeID /* insert only */,
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
										WHERE NEW_VALUES.ProjectId = i.ProjectId
					  FOR XML AUTO, ROOT('Project'))),
			LogData = (SELECT NEW_VALUES.ProjectId
					    FROM inserted AS NEW_VALUES
			           WHERE NEW_VALUES.ProjectId = i.ProjectId
					  FOR XML AUTO, ROOT('Project'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  
	  -- End logging session
	 EXEC dbo.SessionLogUnprepare
END



