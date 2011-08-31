-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-02
-- Description:	Record opportunity changes to AL
-- =============================================
CREATE TRIGGER [dbo].[tr_Opportunity_Log]
   ON  [dbo].[Opportunity]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	IF EXISTS(SELECT 1 FROM deleted)
	BEGIN
	
		DECLARE @PrevPriorityId INT
		DECLARE @PriorityId INT
		DECLARE @NoteText NVARCHAR(2000)
		DECLARE @CurrentPriority NVARCHAR(255)
		DECLARE @PrevPriority NVARCHAR(255)
		DECLARE @OpportunityId INT
		DECLARE @PersonId INT
	
		SELECT @PrevPriority = OP.Priority,
			   @PrevPriorityId = d.PriorityId
		FROM deleted AS d
		INNER JOIN dbo.OpportunityPriorities AS OP ON Op.Id = d.PriorityId
	
		SELECT @PriorityId = i.PriorityId ,
			   @CurrentPriority = OP.Priority,
			   @OpportunityId = i.OpportunityId
		FROM inserted AS i
		INNER JOIN dbo.OpportunityPriorities AS OP ON Op.Id = i.PriorityId
	
		SELECT @PersonId = l.PersonId 
		FROM dbo.SessionLogData AS l 
		WHERE l.SessionID = @@SPID
			-- Determine if the priority was changed
			IF @PrevPriorityId <> @PriorityId
			BEGIN
				-- Create a history record
				SET @NoteText = 'Priority changed.  Was: ' + @PrevPriority + ' now: ' + @CurrentPriority

				INSERT INTO dbo.OpportunityTransition
	            (OpportunityId, OpportunityTransitionStatusId, PersonId, NoteText, TargetPersonId)
				SELECT OpportunityId, 2 /* Notes */, @PersonId, @NoteText, NULL
				FROM inserted 

			END
	END
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.OpportunityId 
			   ,i.[Name]
			   ,i.[ClientId]
			   ,C.Name as 'ClientName'
			   ,i.[SalespersonId]
			   ,S.LastName + ', ' + S.FirstName as 'Salesperson'
			   ,i.[OpportunityStatusId]
			   ,OS.Name AS 'OpportunityStatus'
			   ,OP.[Priority]
			   ,i.[ProjectedStartDate]
			   ,i.[ProjectedEndDate]
			   ,i.[OpportunityNumber]
			   ,i.[Description]
			   ,i.[PracticeId]
			   ,Prac.Name AS 'PracticeName'
			   ,i.[BuyerName]
			   ,i.[CreateDate]
			   ,i.[Pipeline]
			   ,i.[Proposed]
			   ,i.[SendOut]
			   ,i.[ProjectId]
			   ,proj.Name AS 'ProjectName'
			   ,i.[OpportunityIndex]
			   ,i.[EstimatedRevenue]
			   ,i.[OwnerId]
			   ,p.LastName + ', ' + p.FirstName as 'Owner'
			   ,i.[GroupId]
			   ,g.Name 'GroupName'
			   ,i.[LastUpdated]
		  FROM inserted AS i
		       --INNER JOIN v_Opportunity as opp ON i.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityPriorities OP ON i.PriorityId = Op.Id
		       INNER JOIN OpportunityStatus OS ON i.OpportunityStatusId = OS.OpportunityStatusId
		       INNER JOIN Practice Prac ON i.PracticeId = Prac.PracticeId
		       INNER JOIN Person S ON i.SalespersonId = S.PersonId
		       INNER JOIN Client C ON i.ClientId = C.ClientId
		       INNER JOIN Person p ON i.OwnerId = p.PersonId
		       LEFT JOIN Project proj ON i.ProjectId = proj.ProjectId
		       LEFT JOIN ProjectGroup g ON i.GroupId = g.GroupId
	),

	OLD_VALUES AS
	(
		SELECT d.OpportunityId
			   ,d.[Name]
			   ,d.[ClientId]
			   ,C.Name as 'ClientName'
			   ,d.[SalespersonId]
			   ,S.LastName + ', ' + S.FirstName as 'Salesperson'
			   ,d.[OpportunityStatusId]
			   ,os.Name AS 'OpportunityStatus'
			   ,OP.[Priority]
			   ,d.[ProjectedStartDate]
			   ,d.[ProjectedEndDate]
			   ,d.[OpportunityNumber]
			   ,d.[Description]
			   ,d.[PracticeId]
			   ,Prac.Name AS 'PracticeName'
			   ,d.[BuyerName]
			   ,d.[CreateDate]
			   ,d.[Pipeline]
			   ,d.[Proposed]
			   ,d.[SendOut]
			   ,d.[ProjectId]
			   ,proj.Name AS 'ProjectName'
			   ,d.[OpportunityIndex]
			   ,d.[EstimatedRevenue]
			   ,d.[OwnerId]
			   ,p.LastName + ', ' + p.FirstName as 'Owner'
			   ,d.[GroupId]
			   ,g.Name 'GroupName'
			   ,d.[LastUpdated]
		  FROM deleted AS d
		       --INNER JOIN v_Opportunity as opp ON d.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityPriorities OP ON d.PriorityId = Op.Id
		       INNER JOIN OpportunityStatus os ON d.OpportunityStatusId = os.OpportunityStatusId
		       INNER JOIN Practice Prac ON d.PracticeId = Prac.PracticeId
		       INNER JOIN Person S ON d.SalespersonId = S.PersonId
		       INNER JOIN Client C ON d.ClientId = C.ClientId
		       INNER JOIN Person p ON d.OwnerId = p.PersonId
		       LEFT JOIN Project proj ON d.ProjectId = proj.ProjectId
		       LEFT JOIN ProjectGroup g ON d.GroupId = g.GroupId
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
	           WHEN d.OpportunityId IS NULL THEN 3
	           WHEN i.OpportunityId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityId = OLD_VALUES.OpportunityId
			           WHERE NEW_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId) OR OLD_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId)
					  FOR XML AUTO, ROOT('Opportunity'))),
			LogData = (SELECT 
							NEW_VALUES.OpportunityId 
							,NEW_VALUES.[ClientId]
							,NEW_VALUES.[SalespersonId]
							,NEW_VALUES.[OpportunityStatusId]
							,NEW_VALUES.[OpportunityNumber]
							,NEW_VALUES.[PracticeId]
							,NEW_VALUES.[ProjectId]
							,NEW_VALUES.[OpportunityIndex]
							,NEW_VALUES.[OwnerId]
							,NEW_VALUES.[GroupId]
							,NEW_VALUES.[LastUpdated]
							,OLD_VALUES.OpportunityId 
							,OLD_VALUES.[ClientId]
							,OLD_VALUES.[SalespersonId]
							,OLD_VALUES.[OpportunityStatusId]
							,OLD_VALUES.[OpportunityNumber]
							,OLD_VALUES.[PracticeId]
							,OLD_VALUES.[ProjectId]
							,OLD_VALUES.[OpportunityIndex]
							,OLD_VALUES.[OwnerId]
							,OLD_VALUES.[GroupId]
							,OLD_VALUES.[LastUpdated]
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityId = OLD_VALUES.OpportunityId
			           WHERE NEW_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId) OR OLD_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId)
					  FOR XML AUTO, ROOT('Opportunity'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.OpportunityId = d.OpportunityId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		WHERE i.Name <> d.Name
				OR i.ClientId <> d.ClientId
				OR i.SalespersonId <> d.SalespersonId
				OR i.OpportunityStatusId <> d.OpportunityStatusId
				OR i.PracticeId <> d.PracticeId
				OR i.RevenueType <> d.RevenueType
				OR i.PriorityId <> d.PriorityId
				OR (ISNULL(i.OwnerId,0) <> ISNULL(d.OwnerId,0))
				OR (ISNULL(i.GroupId,0) <> ISNULL(d.GroupId,0))
				OR (ISNULL(i.SalespersonId,0) <> ISNULL(d.SalespersonId,0))
				OR (ISNULL(i.Priority,'') <> ISNULL(d.Priority,''))
				OR (ISNULL(i.ProjectedStartDate,'1900-01-01') <> ISNULL(d.ProjectedStartDate,'1900-01-01'))
				OR (ISNULL(i.ProjectedEndDate,'1900-01-01') <> ISNULL(d.ProjectedEndDate,'1900-01-01'))
				OR (ISNULL(i.[Description],'') <> ISNULL(d.[Description],''))
				OR (ISNULL(i.BuyerName,'') <> ISNULL(d.BuyerName,''))
				OR (ISNULL(i.ProjectId,0) <> ISNULL(d.ProjectId,0))
				OR (ISNULL(i.OpportunityIndex,0) <> ISNULL(d.OpportunityIndex,0))
				OR (ISNULL(i.EstimatedRevenue,0) <> ISNULL(d.EstimatedRevenue,0))
				OR (ISNULL(i.OutSideResources,'') <> ISNULL(d.OutSideResources,''))
				OR (ISNULL(i.Pipeline,'') <> ISNULL(d.Pipeline,''))
				OR (ISNULL(i.Proposed,'') <> ISNULL(d.Proposed,''))
				OR (ISNULL(i.SendOut,'') <> ISNULL(d.SendOut,''))
	 --WHERE i.OpportunityId IS NULL -- Deleted record
	 --   OR d.OpportunityId IS NULL -- Added record

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

GO

--EXEC sp_settriggerorder @triggername=N'[dbo].[tr_Opportunity_Log]', @order=N'Last', @stmttype=N'INSERT'
GO

--EXEC sp_settriggerorder @triggername=N'[dbo].[tr_Opportunity_Log]', @order=N'Last', @stmttype=N'UPDATE'
GO



