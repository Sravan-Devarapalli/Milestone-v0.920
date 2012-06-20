﻿CREATE PROCEDURE [dbo].[ActivityLogGetCount]
(
	@StartDate     DATETIME,
	@EndDate       DATETIME,
	@PersonId      INT,
	@ProjectId	   INT = NULL,
	@EventSource   NVARCHAR(50),
	@OpportunityId INT = NULL,
	@MilestoneId   INT = NULL
)
AS
	SET NOCOUNT ON
	/*
	-= NoteTarget table =-
	1	Milestone
	2	Project
	3	Person
	4	Opportunity
	*/

	DECLARE @PersonLastFirstName NVARCHAR(85)

	IF @PersonId IS NOT NULL
	BEGIN
		SELECT @PersonLastFirstName = p.LastName + ', ' + p.FirstName
		FROM Person p
		WHERE p.PersonId = @PersonId
	END

	SELECT COUNT(*) AS NUM
	  FROM dbo.UserActivityLog AS a
	       INNER JOIN dbo.UserActivityType AS t ON a.ActivityTypeID = t.ActivityTypeID
    WHERE CONVERT(DATE,a.LogDate) BETWEEN CONVERT(DATE,@StartDate) AND Convert(DATE,@EndDate)
				  AND(
				  ((@EventSource = 'Error' OR @EventSource = 'All' )AND a.LogData.exist('/Error') = 1)
				  OR ((@EventSource = 'AddedPersons' OR @EventSource = 'All' ) AND a.LogData.exist('/Person') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedPersons' OR @EventSource = 'All' ) AND a.LogData.exist('/Person') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'AddedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'AddedProjects' OR @EventSource = 'All' ) AND a.LogData.exist('/Project') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedProjects' OR @EventSource = 'All' ) AND a.LogData.exist('/Project') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedProjects' OR @EventSource = 'All' ) AND a.LogData.exist('/Project') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'AddedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'TimeEntry' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1)
				  OR ((@EventSource = 'AddedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'AddedSOW' OR @EventSource = 'All' ) AND a.LogData.exist('/ProjectAttachment') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'DeletedSOW' OR @EventSource = 'All' ) AND a.LogData.exist('/ProjectAttachment') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'Exports' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 )
				  OR ((@EventSource = 'ProjectSummaryExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Projects')
				  OR ((@EventSource = 'OpportunitySummaryExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Opportunity')
				  OR ((@EventSource = 'TimeEntryByProjectExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Time Entry By Project')
				  OR ((@EventSource = 'TimeEntryByPersonExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Time Entry By Person')
				  OR ((@EventSource = 'BenchReportExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Bench Report')
				  OR ((@EventSource = 'ConsultantUtilTableExport' OR @EventSource = 'All' ) AND a.LogData.exist('/Export') = 1 AND a.LogData.value('(/Export/NEW_VALUES/@From)[1]', 'NVARCHAR(50)') = 'Consultants Util. Table')
				  OR (@EventSource = 'All' AND a.LogData.exist('/') = 1)
			      OR ((@EventSource = 'Person' OR @EventSource = 'All')
							 AND (a.LogData.exist('/Person') = 1 
								  OR a.LogData.exist('/Roles') = 1 
								 )
							AND 
								(
									(a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId 
									OR a.PersonId = @PersonId
									OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
									OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
									OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
									)
								OR @PersonId IS NULL
											
								)
				     )
				  OR (
						((@EventSource = 'Project'  OR @EventSource = 'All') 
							AND (a.LogData.exist('/Project') = 1 
								OR a.LogData.exist('/ProjectAttachment') = 1
								 )
						)
						AND 
						(@ProjectId IS NULL 
						 OR a.LogData.value('(/Project/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 )
					 )
					 
				  OR (
						(@EventSource = 'Opportunity' OR @EventSource = 'All') 
							AND (a.LogData.exist('/Opportunity') = 1 
								OR a.LogData.exist('/OpportunityTransition') = 1 
								)
							AND (@OpportunityId IS NULL OR 
								a.LogData.value('(/Opportunity/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId OR 
								a.LogData.value('(/OpportunityTransition/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId 
								)
					 ) 
				  OR ((@EventSource = 'Milestone' OR @EventSource = 'All')  
					  AND (a.LogData.exist('/Milestone') = 1 
							)
					  AND(@MilestoneId IS NULL 
					     OR a.LogData.value('(/Milestone/NEW_VALUES/@MilestoneId)[1]', 'int') = @MilestoneId
						 ))
					
				  OR ((@EventSource = 'TargetPerson' OR @EventSource = 'All')	 AND ( @PersonId IS NULL 
															OR a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
															OR a.PersonID = @PersonId
															OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId
														  )
					 )
				  OR ( (@EventSource = 'Logon' OR @EventSource = 'All') AND (a.LogData.exist('/Login') = 1 OR a.LogData.exist('/BecomeUser') = 1) AND a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') NOT LIKE '%locked out%'
					 )
				  OR ( (@EventSource = 'LoginSuccessful' OR @EventSource = 'All') AND (a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') = 'Success')
					 )
				  OR ( (@EventSource = 'LoginError' OR @EventSource = 'All') AND ( (a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') <> 'Success' 
																					AND a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') NOT LIKE '%locked out%')
																				  OR a.LogData.value('(/Error/NEW_VALUES/@SourcePage)[1]', 'NVARCHAR(225)') = 'PracticeManagement/Login.aspx' )
					 )
				  OR ( (@EventSource = 'AccountLockouts' OR @EventSource = 'All') AND (a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') LIKE '%locked out%')
					 )
				  OR ( (@EventSource = 'PasswordResetRequests' OR @EventSource = 'All') AND t.ActivityName = 'changed' AND a.LogData.exist('(/Membership)') = 1  AND 
					  ( 
				  
					  (CASE WHEN  a.LogData.exist('(/Membership)') = 1 
						THEN
								(CASE WHEN 
									CONVERT(XML, a.Data).value('(/Membership/NEW_VALUES/@HashedPassword)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Membership/NEW_VALUES/OLD_VALUES/@HashedPassword)[1]', 'NVARCHAR(100)')
									THEN 1 ELSE 0 END 
								)
						ELSE 0 END) = 1
				  
					  )
				     )
				  OR ( (@EventSource = 'BecomeUser' OR @EventSource = 'All') AND a.LogData.exist('/BecomeUser') = 1
					 )
				  OR ( (@EventSource = 'Security' OR @EventSource = 'All')				 
						AND ( (a.LogData.value('(/Login/NEW_VALUES/@Result)[1]', 'NVARCHAR(100)') LIKE '%locked out%')
																					OR (
																							(CASE WHEN  a.LogData.exist('(/Membership)') = 1 
																								  THEN
																										 (CASE WHEN 
																												CONVERT(XML, a.Data).value('(/Membership/NEW_VALUES/@HashedPassword)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Membership/NEW_VALUES/OLD_VALUES/@HashedPassword)[1]', 'NVARCHAR(100)')
																												THEN 1 ELSE 0 END 
																											)
																								  ELSE 0 END) = 1

																					)
																					OR (a.LogData.exist('/BecomeUser') = 1)
																				)
				     )
				  OR ( (@EventSource = 'Skills' OR @EventSource = 'All') AND (a.LogData.exist('/PersonSkill') = 1 or a.LogData.exist('/PersonIndustry') = 1)
					 )
				  OR ( (@EventSource = 'AddedSkills' OR @EventSource = 'All') AND (a.LogData.exist('/PersonSkill') = 1 or a.LogData.exist('/PersonIndustry') = 1) AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedSkills' OR @EventSource = 'All') AND (a.LogData.exist('/PersonSkill') = 1 or a.LogData.exist('/PersonIndustry') = 1) AND t.ActivityName = 'Changed'
					 )
				  OR ( (@EventSource = 'DeletedSkills' OR @EventSource = 'All') AND (a.LogData.exist('/PersonSkill') = 1 or a.LogData.exist('/PersonIndustry') = 1) AND t.ActivityName = 'Deleted'
					 )
				  OR ( (@EventSource = 'Strawmen' OR @EventSource = 'All') AND a.LogData.exist('/Strawman') = 1
					 )
				  OR ( (@EventSource = 'AddedStrawmen' OR @EventSource = 'All') AND a.LogData.exist('/Strawman') = 1 AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedStrawmen' OR @EventSource = 'All') AND a.LogData.exist('/Strawman') = 1 AND t.ActivityName = 'Changed'
					 )
				  OR ( (@EventSource = 'DeletedStrawmen' OR @EventSource = 'All') AND a.LogData.exist('/Strawman') = 1 AND t.ActivityName = 'Deleted'
					 )
					)
	
					AND (@ProjectId IS NULL 
						 OR a.LogData.value('(/Project/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Milestone/NEW_VALUES/@MilestoneProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/MilestonePerson/NEW_VALUES/@MilestoneProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Note/NEW_VALUES/@ParentTargetId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/ProjectAttachment/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
							)
					AND (@PersonId IS NULL 
						OR a.PersonId = @PersonId
						OR a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId 
						OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectPersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectName)[1]', 'NVARCHAR(85)') = @PersonLastFirstName
						OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
						OR (a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId
							AND a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 3)
						OR a.LogData.value('(/Export/NEW_VALUES/@User)[1]','NVARCHAR(85)') = @PersonLastFirstName
						)

