﻿CREATE TRIGGER [tr_TEH_LogUpdate]
    ON [dbo].[TimeEntryHours]
    AFTER UPDATE
AS 
BEGIN
		-- Ensure the temporary table exists
		EXEC SessionLogPrepare @UserLogin = NULL;
	
		DECLARE @CurrentPMTime DATETIME 
		SET @CurrentPMTime = dbo.InsertingTime()

		;WITH NEW_VALUES AS
		(
			SELECT 1 AS Tag,
				   NULL AS Parent, 
				   ins.[TimeEntryId] AS 'TimeEntryId',
				   ins.Id
				  ,CONVERT(VARCHAR(10), ins.[CreateDate], 101) AS 'CreateDate'
				  ,CONVERT(VARCHAR(10), ins.[ModifiedDate], 101) AS 'ModifiedDate'
				  ,CAST(ins.[ActualHours] as decimal(20,2)) AS 'ActualHours'
				  ,CAST(TE.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
				  ,CC.[TimeTypeId] AS 'TimeTypeId'
				  ,tType.[Name] AS 'TimeTypeName'
				  ,ins.[ModifiedBy] AS 'ModifiedBy'
				  ,TE.[Note] AS 'Note'
				  ,TER.Name AS 'ReviewStatus'
				  ,CASE ins.[IsChargeable] WHEN 1 THEN 'Billable'
											ELSE 'Not Billable' END AS 'IsBillable'
				  ,CONVERT(VARCHAR(10), TE.[ChargeCodeDate], 101) AS 'ChargeCodeDate'
				  ,modp.LastName + ', ' + modp.FirstName AS 'ModifiedByName'
				  ,objp.LastName + ', ' + objp.FirstName AS 'ObjectName'
				  ,objp.PersonId AS 'ObjectPersonId'
				  ,clnt.[Name] AS 'ClientName'
				  ,clnt.[ClientId] AS 'ClientId'
				  ,PG.[GroupId] AS 'ProjectGroupId'
				  ,PG.[Name] AS 'ProjectGroupName'
				  ,proj.[Name] AS 'ProjectName'
				  ,proj.ProjectId AS 'ProjectId'
				  ,clnt.[Code] + ' - ' + PG.[Code] + ' - ' + proj.ProjectNumber + ' - ' + '01 - ' + tType.Code AS 'ChargeCode'
				  ,proj.IsAllowedToShow AS 'IsAllowedToShow'
			FROM inserted AS ins
			INNER JOIN dbo.TimeEntry AS  TE ON TE.TimeEntryId = ins.TimeEntryId AND  TE.IsAutoGenerated = 0
			INNER JOIN dbo.TimeEntryReviewStatus AS TER ON TER.Id = ins.ReviewStatusId
			INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.Person AS modp ON modp.PersonId = ins.ModifiedBy
			INNER JOIN dbo.Person AS objp ON objp.PersonId = TE.PersonId
			INNER JOIN dbo.Project AS proj ON proj.ProjectId = CC.ProjectId
			INNER JOIN dbo.Client AS clnt ON CC.ClientId = clnt.ClientId
			INNER JOIN dbo.TimeType AS tType ON tType.TimeTypeId = CC.TimeTypeId
			INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
		),

		OLD_VALUES AS
		(
			SELECT   1 AS Tag,
				   NULL AS Parent, 
				   del.[TimeEntryId] AS 'TimeEntryId',
				   del.Id
				  ,CONVERT(VARCHAR(10), del.[CreateDate], 101) AS 'CreateDate'
				  ,CONVERT(VARCHAR(10), del.[ModifiedDate], 101) AS 'ModifiedDate'
				  ,CAST(del.[ActualHours] as decimal(20,2)) AS 'ActualHours'
				  ,CAST(TE.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
				  ,CC.[TimeTypeId] AS 'TimeTypeId'
				  ,tType.[Name] AS 'TimeTypeName'
				  ,del.[ModifiedBy] AS 'ModifiedBy'
				  ,TE.[Note] AS 'Note'
				  ,TER.Name AS 'ReviewStatus'
				  ,CASE del.[IsChargeable] WHEN 1 THEN 'Billable'
											ELSE 'Not Billable' END AS 'IsBillable'
				  ,CONVERT(VARCHAR(10), TE.[ChargeCodeDate], 101) AS 'ChargeCodeDate'
				  ,modp.LastName + ', ' + modp.FirstName AS 'ModifiedByName'
				  ,objp.LastName + ', ' + objp.FirstName AS 'ObjectName'
				  ,objp.PersonId AS 'ObjectPersonId'
				  ,clnt.[Name] AS 'ClientName'
				  ,clnt.[ClientId] AS 'ClientId'
				  ,PG.[GroupId] AS 'ProjectGroupId'
				  ,PG.[Name] AS 'ProjectGroupName'
				  ,proj.[Name] AS 'ProjectName'
				  ,proj.ProjectId AS 'ProjectId'
				  ,clnt.[Code] + ' - ' + PG.[Code] + ' - ' + proj.ProjectNumber + ' - ' + '01 - ' + tType.Code AS 'ChargeCode'
				  ,proj.IsAllowedToShow AS 'IsAllowedToShow'
			FROM deleted AS del
			INNER JOIN dbo.TimeEntry AS  TE ON TE.TimeEntryId = del.TimeEntryId
			INNER JOIN dbo.TimeEntryReviewStatus AS TER ON TER.Id = del.ReviewStatusId
			INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.Person AS modp ON modp.PersonId = del.ModifiedBy
			INNER JOIN dbo.Person AS objp ON objp.PersonId = TE.PersonId
			INNER JOIN dbo.Project AS proj ON proj.ProjectId = CC.ProjectId
			INNER JOIN dbo.Client AS clnt ON CC.ClientId = clnt.ClientId
			INNER JOIN dbo.TimeType AS tType ON tType.TimeTypeId = CC.TimeTypeId
			INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
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
		SELECT  4 ActivityTypeID,
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
							INNER JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id
						   WHERE NEW_VALUES.Id = i.Id
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
							INNER JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id
						   WHERE NEW_VALUES.Id = i.Id
						  FOR XML AUTO, ROOT('TimeEntry'), TYPE),
				@CurrentPMTime
		  FROM inserted AS i
 	  	  INNER JOIN dbo.TimeEntry AS  TE ON TE.TimeEntryId = i.TimeEntryId AND  TE.IsAutoGenerated = 0
		  INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	
	 ;WITH NEW_VALUES AS
		(
			SELECT  i.Id
					,i.[TimeEntryId] AS 'TimeEntryId'
					,TE.PersonId AS 'ObjectPersonId'
					,TE.ChargeCodeId AS 'ChargeCodeId'
					,CONVERT(VARCHAR(10), TE.[ChargeCodeDate], 101) AS 'ChargeCodeDate'
					,TE.[Note] AS 'Note'
					,CAST(TE.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
					,TE.IsCorrect
					,TE.IsAutoGenerated
					,CAST(i.[ActualHours] as decimal(20,2)) AS 'ActualHours'
					,CONVERT(VARCHAR(10), i.[CreateDate], 101) AS 'CreateDate'
					,CONVERT(VARCHAR(10), i.[ModifiedDate], 101) AS 'ModifiedDate'
					,i.[ModifiedBy] AS 'ModifiedBy'
					,i.[IsChargeable] AS 'IsChargeable'
					,i.ReviewStatusId AS 'ReviewStatusId'
			FROM inserted AS i
			INNER JOIN dbo.TimeEntry AS TE ON TE.TimeEntryId = i.TimeEntryId
		),

		OLD_VALUES AS
		(
		SELECT  del.Id
				,CAST(del.[ActualHours] as decimal(20,2)) AS 'ActualHours'
			FROM deleted AS del
		)

		INSERT INTO dbo.TimeEntryHistory
					(TimeEntryId  ,
					  PersonId  ,
					  ChargeCodeId  ,
					  ChargeCodeDate ,
					  Note  ,
					  ForecastedHours ,
					  IsCorrect ,
					  IsAutoGenerated ,
					  OldHours    ,
					  ActualHours ,
					  CreateDate ,
					  ModifiedDate,
					  ModifiedBy  ,
					  IsChargeable,
					  ReviewStatusId  ,
					  ActivityTypeId  ,
					  SessionID)
		SELECT  NEW_VALUES.TimeEntryId,
				NEW_VALUES.ObjectPersonId,
				NEW_VALUES.ChargeCodeId,
				NEW_VALUES.ChargeCodeDate,
				NEW_VALUES.Note,
				NEW_VALUES.ForecastedHours,
				NEW_VALUES.IsCorrect,
				NEW_VALUES.IsAutoGenerated,
				OLD_VALUES.ActualHours,
				NEW_VALUES.ActualHours,
				NEW_VALUES.CreateDate,
				NEW_VALUES.ModifiedDate,
				NEW_VALUES.ModifiedBy,
				NEW_VALUES.IsChargeable,
				NEW_VALUES.ReviewStatusId,
				4,
				l.SessionID
		  FROM NEW_VALUES 
		  INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		  INNER JOIN OLD_VALUES ON NEW_VALUES.Id = OLD_VALUES.Id

	
END

