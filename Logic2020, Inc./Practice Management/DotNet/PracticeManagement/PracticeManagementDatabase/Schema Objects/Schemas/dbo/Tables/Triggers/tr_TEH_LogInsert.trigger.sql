﻿CREATE TRIGGER [tr_TEH_LogInsert]
    ON [dbo].[TimeEntryHours]
    FOR INSERT
AS 
BEGIN
    SET NOCOUNT ON
	
	IF EXISTS(SELECT  1 FROM Inserted i
				JOIN TimeEntry TE ON TE.TimeEntryId = i.TimeEntryId WHERE TE.IsAutoGenerated = 0)
	BEGIN
		-- Ensure the temporary table exists
		EXEC SessionLogPrepare @UserLogin = NULL


		DECLARE @CurrentPMTime DATETIME 
		SET @CurrentPMTime = dbo.InsertingTime()

		;WITH NEW_VALUES AS
		(
			SELECT   1 AS Tag,
					NULL AS Parent, 
					i.[TimeEntryId] AS 'TimeEntryId',
					i.Id
					,CONVERT(VARCHAR(10), i.[CreateDate], 101) AS 'CreateDate'
					,CONVERT(VARCHAR(10), i.[ModifiedDate], 101) AS 'ModifiedDate'
					,CAST(i.[ActualHours] as decimal(20,2)) AS 'ActualHours'
					,CAST(TE.[ForecastedHours] as decimal(20,2)) AS 'ForecastedHours'
					,CC.[TimeTypeId] AS 'TimeTypeId'
					,tType.[Name] AS 'TimeTypeName'
					,i.[ModifiedBy] AS 'ModifiedBy'
					,TE.[Note] AS 'Note'
					,TER.Name AS 'ReviewStatus'
					,CASE i.[IsChargeable] WHEN 1 THEN 'Billable'
											ELSE 'Not Billable' END AS 'IsBillable'
					,CONVERT(VARCHAR(10), TE.[ChargeCodeDate], 101) AS 'ChargeCodeDate'
					,modp.LastName + ', ' + modp.FirstName AS 'ModifiedByName'
					,objp.LastName + ', ' + objp.FirstName AS 'ObjectName'
					,objp.PersonId AS 'ObjectPersonId'
					,clnt.[Name] AS 'ClientName'
					,clnt.[ClientId] AS 'ClientId'
					,PG.[Name] AS 'ProjectGroupName'
					,PG.[GroupId] AS 'ProjectGroupId'
					,proj.[Name] AS 'ProjectName'
					,proj.ProjectId AS 'ProjectId'
					,clnt.[Code] + ' - ' + PG.[Code] + ' - ' + proj.ProjectNumber + ' - ' + '01 - ' + tType.Code AS 'ChargeCode'
					,proj.IsAllowedToShow AS 'IsAllowedToShow'
			FROM inserted AS i
			INNER JOIN dbo.TimeEntry AS TE ON TE.TimeEntryId = i.TimeEntryId
			INNER JOIN dbo.TimeEntryReviewStatus AS TER ON TER.Id = i.ReviewStatusId
			INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.Person AS modp ON modp.PersonId = i.ModifiedBy
			INNER JOIN dbo.Person AS objp ON objp.PersonId = TE.PersonId
			INNER JOIN dbo.Project AS proj ON proj.ProjectId = CC.ProjectId
			INNER JOIN dbo.Client AS clnt ON CC.ClientId = clnt.ClientId
			INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
			INNER JOIN dbo.TimeType AS tType ON tType.TimeTypeId = CC.TimeTypeId
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
		SELECT 3 AS ActivityTypeID /* insert only */,
			   l.SessionID,
			   l.SystemUser,
			   l.Workstation,
			   l.ApplicationName,
			   l.UserLogin,
			   l.PersonID,
			   l.LastName,
			   l.FirstName,
			   Data =  CONVERT(NVARCHAR(MAX),(SELECT	NEW_VALUES.Tag,
														NEW_VALUES.Parent, 
														NEW_VALUES.TimeEntryId,
														NEW_VALUES.CreateDate,
														NEW_VALUES.ModifiedDate,
														NEW_VALUES.ActualHours,
														NEW_VALUES.ForecastedHours,
														NEW_VALUES.TimeTypeId,
														NEW_VALUES.TimeTypeName,
														NEW_VALUES.ModifiedBy,
														NEW_VALUES.Note,
														NEW_VALUES.ReviewStatus,
														NEW_VALUES.IsBillable,
														NEW_VALUES.ChargeCodeDate,
														NEW_VALUES.ModifiedByName,
														NEW_VALUES.ObjectName,
														NEW_VALUES.ObjectPersonId,
														NEW_VALUES.ClientName,
														NEW_VALUES.ClientId,
														NEW_VALUES.ProjectGroupName,
														NEW_VALUES.ProjectGroupId,
														NEW_VALUES.ProjectName,
														NEW_VALUES.ProjectId,
														NEW_VALUES.ChargeCode,
														NEW_VALUES.IsAllowedToShow
												FROM NEW_VALUES
												WHERE NEW_VALUES.Id = i.Id
												FOR XML AUTO, ROOT('TimeEntry'))),
						  
						  LogData = 
			
								--  ,NEW_VALUES.[Note] AS 'NEW_VALUES!1!Note'
								--  ,CONVERT(VARCHAR(10), NEW_VALUES.[ChargeCodeDate], 101) AS 'NEW_VALUES!1!ChargeCodeDate'
								--  ,objp.LastName + ', ' + objp.FirstName AS 'NEW_VALUES!1!ObjectName'
								--  ,CC.[ClientId] AS 'NEW_VALUES!1!ClientId'
								--  ,CC.ProjectGroupId AS 'NEW_VALUES!1!ProjectGroupId'
								--  ,CC.ProjectId AS 'NEW_VALUES!1!ProjectId'
							(SELECT NEW_VALUES.Tag,
									NEW_VALUES.Parent, 
									NEW_VALUES.TimeEntryId,
									NEW_VALUES.CreateDate,
									NEW_VALUES.ModifiedDate,
									NEW_VALUES.ObjectPersonId AS PersonId,
									NEW_VALUES.TimeTypeId,
									NEW_VALUES.TimeTypeName,
							        NEW_VALUES.Note,
									NEW_VALUES.ChargeCodeDate,
									NEW_VALUES.ObjectName,
									NEW_VALUES.ClientId,
									NEW_VALUES.ProjectGroupId,
									NEW_VALUES.ProjectId
					         FROM  NEW_VALUES
			           WHERE NEW_VALUES.Id = i.Id
					  FOR XML AUTO, ROOT('TimeEntry'), TYPE),
				@CurrentPMTime
		  FROM inserted AS i
			   INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  

	 END
END

