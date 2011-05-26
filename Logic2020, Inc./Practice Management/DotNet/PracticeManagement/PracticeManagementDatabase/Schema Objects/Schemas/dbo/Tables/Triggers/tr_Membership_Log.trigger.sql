-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[tr_Membership_Log]
   ON  [dbo].[aspnet_Membership]
   AFTER INSERT, UPDATE, DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @GMT NVARCHAR(10) = (SELECT Value FROM Settings WHERE SettingsKey = 'TimeZone')
	DECLARE @CurrentPMTime DATETIME = (CASE WHEN CHARINDEX('-',@GMT) >0 THEN GETUTCDATE() - REPLACE(@GMT,'-','') ELSE 
											GETUTCDATE() + @GMT END)

	;WITH NEW_VALUES AS
	(
		SELECT 
			p.PersonId,
			u.userid,
			p.LastName + ', ' + p.FirstName as 'Name',
			p.Alias as 'Email',
			i.password as 'HashedPassword'
		  FROM inserted AS i
			left join dbo.aspnet_Users as u on i.userid = u.userid
			left join dbo.person as p on p.alias = u.username
	),

	OLD_VALUES AS
	(
		SELECT 
			p.PersonId,
			u.userid,
			p.LastName + ', ' + p.FirstName as 'Name',
			p.Alias as 'Email',
			d.password as 'HashedPassword'
		  FROM deleted AS d
			left join dbo.aspnet_Users as u on d.userid = u.userid
			left join dbo.person as p on p.alias = u.username
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
	SELECT 4 as ActivityTypeID,
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
			           WHERE (NEW_VALUES.userid = ISNULL(i.userid, d.userid)) OR (OLD_VALUES.userid = ISNULL(i.userid, d.userid))
					  FOR XML AUTO, ROOT('Membership'))),
	       LogData = (SELECT 
							NEW_VALUES.PersonId,
							NEW_VALUES.userid,
							NEW_VALUES.Name,
							NEW_VALUES.Email,
							OLD_VALUES.PersonId,
							OLD_VALUES.userid,
							OLD_VALUES.Name,
							OLD_VALUES.Email
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
			           WHERE (NEW_VALUES.userid = ISNULL(i.userid, d.userid)) OR (OLD_VALUES.userid = ISNULL(i.userid, d.userid))
					  FOR XML AUTO, ROOT('Membership'), TYPE),
			LogDate = @CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.userid = d.userid
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END

GO



