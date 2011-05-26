CREATE PROCEDURE [dbo].[UserActivityLogInsert]
	@ActivityTypeID	INT,
	@LogData		NVARCHAR(MAX)
AS
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

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
	             LogData,
				 Data,
				 LogDate)
	SELECT @ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
		   LogData = (CASE WHEN @ActivityTypeID = 20 THEN '<Error></Error>' ELSE @LogData END), -- if @ActivityTypeID = 20 it is an Error.
	       Data = @LogData,
	       LogDate = @CurrentPMTime
	  FROM dbo.SessionLogData AS l
	  WHERE l.SessionID = @@SPID

	  -- End logging session
	 EXEC dbo.SessionLogUnprepare



GO



