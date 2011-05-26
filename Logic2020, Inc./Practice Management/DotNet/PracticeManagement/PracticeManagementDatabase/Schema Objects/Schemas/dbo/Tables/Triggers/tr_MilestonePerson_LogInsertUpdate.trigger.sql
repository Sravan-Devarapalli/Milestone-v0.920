-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-27-2008
-- Updated by:	
-- Update date:	
-- Description:	Logs the changes in the dbo.Milestone table.
-- =============================================
CREATE TRIGGER [dbo].[tr_MilestonePerson_LogInsertUpdate]
ON [dbo].[MilestonePersonEntry]
AFTER INSERT
AS
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT mp.ProjectId AS MilestoneProjectId,
		       mp.ProjectName,
		       mp.MilestoneId,
		       mp.MilestoneName AS Name,
		       mp.MilestonePersonId,
		       mp.LastName + ', ' + mp.FirstName AS FullName,
		       CONVERT(NVARCHAR(10), mp.StartDate, 101) AS StartDate,
		       CONVERT(NVARCHAR(10), mp.EndDate, 101) AS EndDate,
		       mp.RoleName,
		       mp.Amount,
		       mp.HoursPerDay		       
		  FROM inserted AS i
		       INNER JOIN dbo.v_MilestonePerson AS mp
		           ON i.MilestonePersonId = mp.MilestonePersonId AND i.StartDate = mp.StartDate
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
	SELECT 4 AS ActivityTypeID,
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
			           WHERE NEW_VALUES.MilestonePersonId = mp.MilestonePersonId
	                     AND NEW_VALUES.StartDate = mp.StartDate
					  FOR XML AUTO, ROOT('MilestonePerson'))),
		  LogData = (SELECT NEW_VALUES.MilestoneProjectId,
							NEW_VALUES.ProjectName,
							NEW_VALUES.MilestoneId,
							NEW_VALUES.MilestonePersonId
					    FROM NEW_VALUES
			           WHERE NEW_VALUES.MilestonePersonId = mp.MilestonePersonId
	                     AND NEW_VALUES.StartDate = mp.StartDate
					  FOR XML AUTO, ROOT('MilestonePerson'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       INNER JOIN dbo.v_MilestonePerson AS mp
	           ON i.MilestonePersonId = mp.MilestonePersonId AND i.StartDate = mp.StartDate
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END

GO



