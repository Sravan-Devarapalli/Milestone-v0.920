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

	;WITH NEW_VALUES AS
	(
		SELECT i.OpportunityId 
			   ,i.[Name]
			   ,i.[ClientId]
			   ,opp.ClientName
			   ,i.[SalespersonId]
			   ,opp.SalespersonFirstName + ', ' + opp.SalespersonLastName as 'Salesperson'
			   ,i.[OpportunityStatusId]
			   ,opp.OpportunityStatusName
			   ,OP.[Priority]
			   ,i.[ProjectedStartDate]
			   ,i.[ProjectedEndDate]
			   ,i.[OpportunityNumber]
			   ,i.[Description]
			   ,i.[PracticeId]
			   ,opp.PracticeName
			   ,i.[BuyerName]
			   ,i.[CreateDate]
			   ,i.[Pipeline]
			   ,i.[Proposed]
			   ,i.[SendOut]
			   ,i.[ProjectId]
			   ,i.[OpportunityIndex]
			   ,i.[RevenueType]
			   ,i.[OwnerId]
			   ,i.[GroupId]
			   ,opp.GroupName
			   ,i.[LastUpdated]
		  FROM inserted AS i
		       INNER JOIN v_Opportunity as opp ON i.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityPriorities OP ON i.PriorityId = Op.Id
	),

	OLD_VALUES AS
	(
		SELECT d.OpportunityId
			   ,d.[Name]
			   ,d.[ClientId]
			   ,opp.ClientName
			   ,d.[SalespersonId]
			   ,opp.SalespersonFirstName + ', ' + opp.SalespersonLastName as 'Salesperson'
			   ,d.[OpportunityStatusId]
			   ,opp.OpportunityStatusName
			   ,OP.[Priority]
			   ,d.[ProjectedStartDate]
			   ,d.[ProjectedEndDate]
			   ,d.[OpportunityNumber]
			   ,d.[Description]
			   ,d.[PracticeId]
			   ,opp.PracticeName
			   ,d.[BuyerName]
			   ,d.[CreateDate]
			   ,d.[Pipeline]
			   ,d.[Proposed]
			   ,d.[SendOut]
			   ,d.[ProjectId]
			   ,d.[OpportunityIndex]
			   ,d.[RevenueType]
			   ,d.[OwnerId]
			   ,d.[GroupId]
			   ,opp.GroupName
			   ,d.[LastUpdated]
		  FROM deleted AS d
		       INNER JOIN v_Opportunity as opp ON d.OpportunityId = opp.OpportunityId
		       INNER JOIN OpportunityPriorities OP ON d.PriorityId = Op.Id
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
							,NEW_VALUES.[RevenueType]
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
							,OLD_VALUES.[RevenueType]
							,OLD_VALUES.[OwnerId]
							,OLD_VALUES.[GroupId]
							,OLD_VALUES.[LastUpdated]
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.OpportunityId = OLD_VALUES.OpportunityId
			           WHERE NEW_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId) OR OLD_VALUES.OpportunityId = ISNULL(i.OpportunityId, d.OpportunityId)
					  FOR XML AUTO, ROOT('Opportunity'), TYPE)
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.OpportunityId = d.OpportunityId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 --WHERE i.OpportunityId IS NULL -- Deleted record
	 --   OR d.OpportunityId IS NULL -- Added record

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

GO
EXECUTE sp_settriggerorder @triggername = N'[dbo].[tr_Opportunity_Log]', @order = N'last', @stmttype = N'insert';


GO
EXECUTE sp_settriggerorder @triggername = N'[dbo].[tr_Opportunity_Log]', @order = N'last', @stmttype = N'update';


