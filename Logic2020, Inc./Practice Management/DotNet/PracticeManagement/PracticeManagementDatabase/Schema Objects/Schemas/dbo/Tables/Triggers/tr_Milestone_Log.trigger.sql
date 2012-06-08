﻿-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-27-2008
-- Updated by:	
-- Update date:	
-- Description:	Logs the changes in the dbo.Milestone table.
-- =============================================
CREATE TRIGGER [dbo].[tr_Milestone_Log]
ON [dbo].[Milestone]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.MilestoneId,
		       i.Description AS Name,
		       i.ProjectId AS MilestoneProjectId,
		       p.Name AS ProjectName,
		       i.ProjectedDeliveryDate,
		       i.StartDate,
		       i.Amount,
		       i.ConsultantsCanAdjust,
		       i.IsChargeable,
		       i.IsHourlyAmount	       
		  FROM inserted AS i
		       INNER JOIN dbo.Project AS p ON i.ProjectId = p.ProjectId
	),

	OLD_VALUES AS
	(
		SELECT d.MilestoneId,
		       d.Description AS Name,
		       d.ProjectId AS MilestoneProjectId,
		       p.Name AS ProjectName,
		       d.ProjectedDeliveryDate,
		       d.StartDate,
		       d.Amount,
		       d.ConsultantsCanAdjust,
		       d.IsChargeable,
		       d.IsHourlyAmount	
		  FROM deleted AS d
		       INNER JOIN dbo.Project AS p ON d.ProjectId = p.ProjectId
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
	           WHEN d.MilestoneId IS NULL THEN 3
	           WHEN i.MilestoneId IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.MilestoneId = OLD_VALUES.MilestoneId
			           WHERE NEW_VALUES.MilestoneId = ISNULL(i.MilestoneId, d.MilestoneId) OR OLD_VALUES.MilestoneId = ISNULL(i.MilestoneId, d.MilestoneId)
					  FOR XML AUTO, ROOT('Milestone'))),
			LogData = (SELECT   NEW_VALUES.MilestoneId,
								NEW_VALUES.MilestoneProjectId,
								NEW_VALUES.ProjectedDeliveryDate,
								NEW_VALUES.StartDate,
								OLD_VALUES.MilestoneId,
								OLD_VALUES.MilestoneProjectId,
								OLD_VALUES.ProjectedDeliveryDate,
								OLD_VALUES.StartDate
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.MilestoneId = OLD_VALUES.MilestoneId
			           WHERE NEW_VALUES.MilestoneId = ISNULL(i.MilestoneId, d.MilestoneId) OR OLD_VALUES.MilestoneId = ISNULL(i.MilestoneId, d.MilestoneId)
					  FOR XML AUTO, ROOT('Milestone'), TYPE),
			@CurrentPMTime

	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.MilestoneId = d.MilestoneId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 WHERE i.MilestoneId IS NULL -- Deleted record
	    OR d.MilestoneId IS NULL -- Added record
	    OR ISNULL(i.Description, '') <> ISNULL(d.Description, '')
	    OR ISNULL(i.Amount, 0) <> ISNULL(d.Amount, 0)
	    OR i.StartDate <> d.StartDate
	    OR i.ProjectedDeliveryDate <> d.ProjectedDeliveryDate
	    OR i.IsHourlyAmount <> d.IsHourlyAmount

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END

GO



