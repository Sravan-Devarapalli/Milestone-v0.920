﻿CREATE TRIGGER [dbo].[tr_TE_LogUpdate]
    ON [dbo].[TimeEntry]
    AFTER UPDATE
AS 
BEGIN

	IF NOT EXISTS (SELECT  1 FROM inserted WHERE IsAutoGenerated = 1 )
	BEGIN
		-- Ensure the temporary table exists
		EXEC SessionLogPrepare @UserLogin = NULL;

		;WITH UALog AS
		(
		    SELECT RANK() OVER(PARTITION BY  CAST(u.Data as XML).value('(/TimeEntry/NEW_VALUES/@TimeEntryId)[1]', 'int')  ORDER BY ActivityId DESC) AS RANKnO,
			       CAST(u.Data as XML) as XmlData,
				   CAST(u.Data as XML).value('(/TimeEntry/NEW_VALUES/@TimeEntryId)[1]', 'int') TimeEntryId,
				   u.ActivityID
			 FROM dbo.UserActivityLog AS u 
			 INNER JOIN inserted i ON  u.ActivityTypeID = 4 AND i.TimeEntryId = CAST(u.Data as XML).value('(/TimeEntry/NEW_VALUES/@TimeEntryId)[1]', 'int')
			 INNER JOIN dbo.SessionLogData  AS l ON l.SessionID = @@SPID and l.SessionID = u.SessionID 
		)
		,NEW_VALUES AS
		(
			SELECT  1 AS Tag,
				   NULL AS Parent, 
				   i.[TimeEntryId] AS 'TimeEntryId',
				    uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@CreateDate)[1]', 'VARCHAR(10)') AS 'CreateDate',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ModifiedDate)[1]', 'VARCHAR(10)') AS 'ModifiedDate',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ActualHours)[1]', 'decimal(20,2)') AS 'ActualHours',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ForeCASTedHours)[1]', 'decimal(20,2)') AS 'ForeCASTedHours',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@TimeTypeId)[1]', 'int') AS 'TimeTypeId',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@TimeTypeName)[1]', 'VARCHAR(1000)') AS 'TimeTypeName',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ModifiedBy)[1]', 'int') AS 'ModifiedBy',
					i.[Note] AS 'Note',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ReviewStatus)[1]', 'VARCHAR(1000)') AS 'ReviewStatus',
					CASE i.[IsCorrect] WHEN 1 THEN 'Correct'
										 ELSE 'InCorrect' END AS 'IsCorrect',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@IsBillable)[1]', 'VARCHAR(1000)') AS 'IsBillable',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ChargeCodeDate)[1]', 'VARCHAR(10)') AS 'ChargeCodeDate',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ModifiedByName)[1]', 'VARCHAR(1000)') AS 'ModifiedByName',
			     uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ObjectName)[1]', 'VARCHAR(1000)') AS 'ObjectName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ObjectPersonId)[1]', 'INT') AS 'ObjectPersonId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ClientName)[1]', 'VARCHAR(1000)') AS 'ClientName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ClientId)[1]', 'INT') AS 'ClientId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ProjectGroupId)[1]', 'INT') AS 'ProjectGroupId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ProjectGroupName)[1]', 'VARCHAR(1000)') AS 'ProjectGroupName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ProjectName)[1]', 'VARCHAR(1000)') AS 'ProjectName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ProjectId)[1]', 'INT') AS 'ProjectId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/@ChargeCode)[1]', 'VARCHAR(1000)') AS 'ChargeCode'
			FROM inserted AS i
			INNER JOIN UALog ON i.TimeEntryId = uaLog.TimeEntryId and UALog.RANKnO = 1 
		),

		OLD_VALUES AS
		(SELECT 1 AS Tag,
				   NULL AS Parent, 
				   del.[TimeEntryId] AS 'TimeEntryId',
				    uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@CreateDate)[1]', 'VARCHAR(10)') AS 'CreateDate',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ModifiedDate)[1]', 'VARCHAR(10)') AS 'ModifiedDate',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ActualHours)[1]', 'decimal(20,2)') AS 'ActualHours',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ForeCASTedHours)[1]', 'decimal(20,2)') AS 'ForeCASTedHours',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@TimeTypeId)[1]', 'int') AS 'TimeTypeId',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@TimeTypeName)[1]', 'VARCHAR(1000)') AS 'TimeTypeName',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ModifiedBy)[1]', 'int') AS 'ModifiedBy',
					del.[Note] AS 'Note',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ReviewStatus)[1]', 'VARCHAR(1000)') AS 'ReviewStatus',
					CASE del.[IsCorrect] WHEN 1 THEN 'Correct'
										 ELSE 'InCorrect' END AS 'IsCorrect',
					uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@IsBillable)[1]', 'VARCHAR(1000)') AS 'IsBillable',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ChargeCodeDate)[1]', 'VARCHAR(10)') AS 'ChargeCodeDate',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ModifiedByName)[1]', 'VARCHAR(1000)') AS 'ModifiedByName',
			     uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ObjectName)[1]', 'VARCHAR(1000)') AS 'ObjectName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ObjectPersonId)[1]', 'INT') AS 'ObjectPersonId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ClientName)[1]', 'VARCHAR(1000)') AS 'ClientName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ClientId)[1]', 'INT') AS 'ClientId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ProjectGroupId)[1]', 'INT') AS 'ProjectGroupId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ProjectGroupName)[1]', 'VARCHAR(1000)') AS 'ProjectGroupName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ProjectName)[1]', 'VARCHAR(1000)') AS 'ProjectName',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ProjectId)[1]', 'INT') AS 'ProjectId',
				 uaLog.XmlData.value('(/TimeEntry/NEW_VALUES/OLD_VALUES/@ChargeCode)[1]', 'VARCHAR(1000)') AS 'ChargeCode'
			FROM deleted AS del
			INNER JOIN UALog ON del.TimeEntryId = uaLog.TimeEntryId and UALog.RANKnO = 1 
		)

		UPDATE u
		SET Data = CONVERT(NVARCHAR(MAX),(SELECT *
							FROM NEW_VALUES
							INNER JOIN OLD_VALUES ON NEW_VALUES.TimeEntryId = OLD_VALUES.TimeEntryId
						   WHERE NEW_VALUES.TimeEntryId = ualog.TimeEntryId
						  FOR XML AUTO, ROOT('TimeEntry'))),
			LogData = (SELECT NEW_VALUES.Parent, 
									NEW_VALUES.TimeEntryId,
									NEW_VALUES.CreateDate,
									NEW_VALUES.ModifiedDate,
									NEW_VALUES.TimeTypeId,
									NEW_VALUES.ModifiedBy,
									NEW_VALUES.ChargeCodeDate,
									NEW_VALUES.ObjectPersonId,
									NEW_VALUES.ClientId,
									NEW_VALUES.ProjectGroupId,
									NEW_VALUES.ProjectId,
									OLD_VALUES.Parent, 
									OLD_VALUES.TimeEntryId,
									OLD_VALUES.CreateDate,
									OLD_VALUES.ModifiedDate,
									OLD_VALUES.TimeTypeId,
									OLD_VALUES.ModifiedBy,
									OLD_VALUES.ChargeCodeDate,
									OLD_VALUES.ObjectPersonId,
									OLD_VALUES.ClientId,
									OLD_VALUES.ProjectGroupId,
									OLD_VALUES.ProjectId
							FROM NEW_VALUES
							INNER JOIN OLD_VALUES ON NEW_VALUES.TimeEntryId = OLD_VALUES.TimeEntryId
						   WHERE NEW_VALUES.TimeEntryId = ualog.TimeEntryId
						  FOR XML AUTO, ROOT('TimeEntry'), TYPE)
		  FROM UALog AS ualog
		  INNER JOIN dbo.UserActivityLog AS u ON  u.ActivityID = ualog.ActivityID AND  UALog.RANKnO = 1 
	 
	 END
END

GO



