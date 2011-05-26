-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[tr_PersonRoles_Log]
   ON  [dbo].[aspnet_UsersInRoles]
   AFTER INSERT, UPDATE, DELETE
as
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()
	
	;WITH NEW_VALUES AS
	(
		SELECT 
			p.PersonId,
			u.userid,
			r.roleid,
			p.LastName + ', ' + p.FirstName as 'Name',
			p.Alias as 'Email',
			r.RoleName as 'Role',
			'Set' as 'Action'
		  FROM inserted AS i
			left join dbo.aspnet_Roles as r on i.roleid = r.roleid
			left join dbo.aspnet_Users as u on i.userid = u.userid
			left join dbo.person as p on p.alias = u.username
	),

	OLD_VALUES AS
	(
		SELECT 
			p.PersonId,
			u.userid,
			r.roleid,
			p.LastName + ', ' + p.FirstName as 'Name',
			p.Alias as 'Email',
			r.RoleName as 'Role',
			'Unset' as 'Action'
		  FROM deleted AS d
			left join dbo.aspnet_Roles as r on d.roleid = r.roleid
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
	       Data = CONVERT(NVARCHAR(MAX),(SELECT NEW_VALUES.PersonId, NEW_VALUES.[Name], NEW_VALUES.[Role], NEW_VALUES.[Action],
							 OLD_VALUES.PersonId, OLD_VALUES.[Name], OLD_VALUES.[Role], OLD_VALUES.[Action]
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
			           WHERE (NEW_VALUES.userid = ISNULL(i.userid, d.userid) AND new_values.roleid=i.roleid) 
								OR (OLD_VALUES.userid = ISNULL(i.userid, d.userid) AND OLD_VALUES.roleid = d.roleid)
					  FOR XML AUTO, ROOT('Roles'))),
			LogData = (SELECT NEW_VALUES.PersonId,NEW_VALUES.[Action],
							 OLD_VALUES.PersonId,OLD_VALUES.[Action]
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonID = OLD_VALUES.PersonID
			           WHERE (NEW_VALUES.userid = ISNULL(i.userid, d.userid) AND new_values.roleid=i.roleid) 
								OR (OLD_VALUES.userid = ISNULL(i.userid, d.userid) AND OLD_VALUES.roleid = d.roleid)
					  FOR XML AUTO, ROOT('Roles'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.userid = d.userid
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	 -- End logging session
	 EXEC dbo.SessionLogUnprepare
END


GO



