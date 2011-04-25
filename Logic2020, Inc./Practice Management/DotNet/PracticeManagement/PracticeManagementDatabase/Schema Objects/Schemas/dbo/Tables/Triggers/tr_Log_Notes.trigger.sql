-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-08
-- Description:	Adds notes to AL
-- =============================================
CREATE TRIGGER [dbo].[tr_Log_Notes] 
   ON  [dbo].[Note] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	 --Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	;WITH NEW_VALUES AS
	(
		SELECT  i.[NoteId]
			   ,i.[PersonId]
			   ,n.LastName + ', ' + n.FirstName as 'By'
			   ,i.[CreateDate]
			   ,i.[NoteText]
			   ,i.TargetId
			   ,n.NoteTargetId
			   ,n.NoteTargetName
			   ,m.ProjectId as ParentTargetId
		FROM inserted AS i
			inner join v_Notes as n on n.NoteId = i.NoteId	
			left join dbo.Milestone m on n.NoteTargetId = 1 AND m.MilestoneId = i.TargetId
	),

	OLD_VALUES AS
	(
		SELECT  d.[NoteId]
			   ,d.[PersonId]
			   ,n.LastName + ', ' + n.FirstName as 'By'
			   ,d.[CreateDate]
			   ,d.[NoteText]
			   ,d.TargetId
			   ,n.NoteTargetId
			   ,n.NoteTargetName
			   ,m.ProjectId as ParentTargetId
		  FROM deleted AS d
			inner join v_Notes as n on n.NoteId = d.NoteId	
			left join dbo.Milestone m on n.NoteTargetId = 1 AND m.MilestoneId = d.TargetId
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
	           WHEN d.[NoteId] IS NULL THEN 3
	           WHEN i.[NoteId] IS NULL THEN 5
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.[NoteId] = OLD_VALUES.[NoteId]
			           WHERE NEW_VALUES.[NoteId] = ISNULL(i.[NoteId], d.[NoteId]) OR OLD_VALUES.[NoteId] = ISNULL(i.[NoteId], d.[NoteId])
					  FOR XML AUTO, ROOT('Note'))),
		  LogData = (
						SELECT 
							NEW_VALUES.[NoteId],
							NEW_VALUES.[PersonId],
							NEW_VALUES.[By],
							NEW_VALUES.[CreateDate],
							NEW_VALUES.[TargetId],
							NEW_VALUES.[NoteTargetId],
							NEW_VALUES.[NoteTargetName],
							NEW_VALUES.[ParentTargetId],
							OLD_VALUES.[NoteId],
							OLD_VALUES.[PersonId],
							OLD_VALUES.[By],
							OLD_VALUES.[CreateDate],
							OLD_VALUES.[TargetId],
							OLD_VALUES.[NoteTargetId],
							OLD_VALUES.[NoteTargetName],
							OLD_VALUES.[ParentTargetId]
						FROM NEW_VALUES
								FULL JOIN OLD_VALUES ON NEW_VALUES.[NoteId] = OLD_VALUES.[NoteId]
						WHERE NEW_VALUES.[NoteId] = ISNULL(i.[NoteId], d.[NoteId]) OR OLD_VALUES.[NoteId] = ISNULL(i.[NoteId], d.[NoteId])
						FOR XML AUTO, ROOT('Note'), TYPE
					)
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.[NoteId] = d.[NoteId]
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END

