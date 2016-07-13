CREATE PROCEDURE [dbo].[ActivityLogGetCount]
(
	@StartDate     DATETIME,
	@EndDate       DATETIME,
	@PersonId      INT,
	@ProjectId	   INT = NULL,
	@VendorId      INT = NULL,
	@EventSource   NVARCHAR(50),
	@OpportunityId INT = NULL,
	@MilestoneId   INT = NULL,
	@PracticeAreas BIT = 1,
	@SOWBudget     BIT = 1,
	@ClientDirector BIT = 1,
	@POAmount	   BIT = 1,
	@Capabilities  BIT = 1,
	@NewOrExtension BIT = 1,
	@PONumber      BIT = 1,
	@ProjectStatus BIT = 1,
	@SalesPerson   BIT = 1,
	@ProjectOwner  BIT = 1,
	@division      BIT = 1,
	@Channel       BIT = 1,
	@Offering	   BIT = 1,
	@RevenueType   BIT = 1
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
    WHERE CONVERT(DATE,a.LogDate) BETWEEN CONVERT(DATE,@StartDate) AND CONVERT(DATE,@EndDate)
				  AND(
				  ((@EventSource = 'Error' OR @EventSource = 'All' )AND a.LogData.exist('/Error') = 1)
				  OR ((@EventSource = 'AddedPersons' OR @EventSource = 'All' ) AND a.LogData.exist('/Person') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedPersons' OR @EventSource = 'All' ) AND a.LogData.exist('/Person') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'AddedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedOpportunities' OR @EventSource = 'All' ) AND a.LogData.exist('/Opportunity') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'AddedProjects' OR @EventSource = 'All' ) AND (a.LogData.exist('/Project') = 1 OR a.LogData.exist('/Attribution') = 1)  AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedProjects' OR @EventSource = 'All' ) AND (a.LogData.exist('/Project') = 1 OR a.LogData.exist('/Attribution') = 1)  AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedProjects' OR @EventSource = 'All' ) AND (a.LogData.exist('/Project') = 1 OR a.LogData.exist('/Attribution') = 1)  AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'AddedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedMilestones' OR @EventSource = 'All' ) AND a.LogData.exist('/Milestone') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'TimeEntry' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1)
				  OR ((@EventSource = 'AddedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedTimeEntries' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1 AND t.ActivityName = 'Deleted')
				  OR ((@EventSource = 'Calendar' OR @EventSource = 'All' ) AND (a.LogData.exist('/PersonCalendar') = 1 OR a.LogData.exist('/CompanyHoliday') = 1 OR a.LogData.exist('/SubstituteHoliday') = 1 OR a.LogData.exist('/CompanyWorkingDay') = 1))
				  OR ((@EventSource = 'AddedCalendar' OR @EventSource = 'All' ) AND (a.LogData.exist('/PersonCalendar') = 1 OR a.LogData.exist('/CompanyHoliday') = 1 OR a.LogData.exist('/SubstituteHoliday') = 1 OR a.LogData.exist('/CompanyWorkingDay') = 1) AND t.ActivityName = 'Added')
				  OR ((@EventSource = 'ChangedCalendar' OR @EventSource = 'All' ) AND (a.LogData.exist('/PersonCalendar') = 1 OR a.LogData.exist('/CompanyHoliday') = 1 OR a.LogData.exist('/SubstituteHoliday') = 1 OR a.LogData.exist('/CompanyWorkingDay') = 1) AND t.ActivityName = 'Changed')
				  OR ((@EventSource = 'DeletedCalendar' OR @EventSource = 'All' ) AND (a.LogData.exist('/PersonCalendar') = 1 OR a.LogData.exist('/CompanyHoliday') = 1 OR a.LogData.exist('/SubstituteHoliday') = 1 OR a.LogData.exist('/CompanyWorkingDay') = 1) AND t.ActivityName = 'Deleted')

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
								OR a.LogData.exist('/Attribution') = 1
								 )
						)
						AND 
						(@ProjectId IS NULL 
						OR (a.LogData.value('(/Project/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId 
							AND ( ((CASE WHEN (a.LogData.exist('(/Project)') = 1) AND @PracticeAreas = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@PracticeName)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@PracticeName)[1]', 'NVARCHAR(100)')
									   THEN 1 
									   ELSE 0 END) +
									   (CASE WHEN (CONVERT(XML,a.Data).exist('(/Project)') = 1) AND @division = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@Division)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@Division)[1]', 'NVARCHAR(100)')
											   THEN 1 
											   ELSE 0 END) +
											   (CASE WHEN (CONVERT(XML,a.Data).exist('(/Project)') = 1) AND @RevenueType = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@RevenueType)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@RevenueType)[1]', 'NVARCHAR(100)')
											   THEN 1 
											   ELSE 0 END) +
											   (CASE WHEN (CONVERT(XML,a.Data).exist('(/Project)') = 1) AND @Channel = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@Channel)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@Channel)[1]', 'NVARCHAR(100)')
											   THEN 1 
											   ELSE 0 END) +
											    (CASE WHEN (CONVERT(XML,a.Data).exist('(/Project)') = 1) AND @Offering = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@Offering)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@Offering)[1]', 'NVARCHAR(100)')
											   THEN 1 
											   ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @SOWBudget = 1 AND ISNULL(CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@SowBudget)[1]', 'DECIMAL(18,2)'),-1) <> ISNULL(CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@SowBudget)[1]', 'DECIMAL(18,2)'),-1)
									   THEN 1
									    ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 and @ClientDirector = 1 and CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@ExecutiveInCharge)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@ExecutiveInCharge)[1]', 'NVARCHAR(100)')
									   THEN 1
									    
									ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @POAmount = 1 AND 
									    (isnull(CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@POAmount)[1]', 'DECIMAL(18,2)'),-1) <> isnull(CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@POAmount)[1]', 'DECIMAL(18,2)'),-1))
									   THEN 1
									ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @NewOrExtension = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@BusinessType)[1]', 'NVARCHAR(50)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@BusinessType)[1]', 'NVARCHAR(50)')
									   THEN 1
									 
									ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @PONumber = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@PONumber)[1]', 'NVARCHAR(50)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@PONumber)[1]', 'NVARCHAR(50)')
									   THEN 1
									  
									ELSE 0 END) + 
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @ProjectStatus = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@ProjectStatusName)[1]', 'NVARCHAR(20)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@ProjectStatusName)[1]', 'NVARCHAR(20)')
									   THEN 1
									
									ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @SalesPerson = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@SalesPerson)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@SalesPerson)[1]', 'NVARCHAR(100)')
									   THEN 1
									
									ELSE 0 END) +
									  (CASE WHEN a.LogData.exist('(/Project)') = 1 AND @ProjectOwner = 1 AND CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/@ProjectManager)[1]', 'NVARCHAR(100)') <> CONVERT(XML, a.Data).value('(/Project/NEW_VALUES/OLD_VALUES/@ProjectManager)[1]', 'NVARCHAR(100)')
									   THEN 1
									
									   ELSE 0 END)) > 0
									   )
							  )
						OR a.LogData.value('(/ProjectAttachment/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						OR a.LogData.value('(/ProjectAttachment/NEW_VALUES/OLD_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						OR a.LogData.value('(/Attribution/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						OR a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 )
					 )

					 OR((@EventSource = 'Vendor' OR @EventSource = 'All')	 AND ( @VendorId IS NULL 
															OR a.LogData.value('(/*/*/@VendorId)[1]', 'int') = @VendorId
															OR a.LogData.value('(/*/*/*/@VendorId)[1]', 'int') = @VendorId))
					 
				  OR (
						(@EventSource = 'Opportunity' OR @EventSource = 'All') 
							AND (a.LogData.exist('/Opportunity') = 1 
								OR a.LogData.exist('/OpportunityTransition') = 1 
								)
							AND (	@OpportunityId IS NULL 
									OR a.LogData.value('(/Opportunity/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId 
									OR a.LogData.value('(/OpportunityTransition/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId 
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
															OR (a.LogData.value('(/Attribution/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId AND a.LogData.value('(/Attribution/NEW_VALUES/@AttributionRecordTypeId)[1]', 'int') = 1) --Person Type
															OR (a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@TargetId)[1]', 'int') = @PersonId AND a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@AttributionRecordTypeId)[1]', 'int') = 1) --Person Type
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
				  OR ( (@EventSource = 'Practice' OR @EventSource = 'All') AND a.LogData.exist('/Practice') = 1
					 )
				  OR ( (@EventSource = 'AddedPractice' OR @EventSource = 'All') AND a.LogData.exist('/Practice') = 1 AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedPractice' OR @EventSource = 'All') AND a.LogData.exist('/Practice') = 1 AND t.ActivityName = 'Changed'
					 )
				  OR ( (@EventSource = 'DeletedPractice' OR @EventSource = 'All') AND a.LogData.exist('/Practice') = 1 AND t.ActivityName = 'Deleted'
					 )
				  OR ( (@EventSource = 'PracticeCapability' OR @EventSource = 'All') AND a.LogData.exist('/PracticeCapability') = 1
					 )
				  OR ( (@EventSource = 'AddedPracticeCapability' OR @EventSource = 'All') AND a.LogData.exist('/PracticeCapability') = 1 AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedPracticeCapability' OR @EventSource = 'All') AND a.LogData.exist('/PracticeCapability') = 1 AND t.ActivityName = 'Changed'
					 )
				  OR ( (@EventSource = 'DeletedPracticeCapability' OR @EventSource = 'All') AND a.LogData.exist('/PracticeCapability') = 1 AND t.ActivityName = 'Deleted'
					 )
				  OR ( (@EventSource = 'Title' OR @EventSource = 'All') AND a.LogData.exist('/Title') = 1
					 )
				  OR ( (@EventSource = 'AddedTitle' OR @EventSource = 'All') AND a.LogData.exist('/Title') = 1 AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedTitle' OR @EventSource = 'All') AND a.LogData.exist('/Title') = 1 AND t.ActivityName = 'Changed'
					 )
				  OR ( (@EventSource = 'DeletedTitle' OR @EventSource = 'All') AND a.LogData.exist('/Title') = 1 AND t.ActivityName = 'Deleted'
					 )
				  OR ( (@EventSource = 'PerformanceManagement' OR @EventSource = 'All') AND a.LogData.exist('/ProjectFeedback') = 1
					 )
				  OR ( (@EventSource = 'AddedFeedback' OR @EventSource = 'All') AND a.LogData.exist('/ProjectFeedback') = 1 AND t.ActivityName = 'Added'
					 )
				  OR ( (@EventSource = 'ChangedFeedback' OR @EventSource = 'All') AND a.LogData.exist('/ProjectFeedback') = 1 AND t.ActivityName = 'Changed'
					 )
			      OR ( (@EventSource = 'DeletedFeedback' OR @EventSource = 'All') AND a.LogData.exist('/ProjectFeedback') = 1 AND t.ActivityName = 'Deleted'
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
						OR a.LogData.value('(/Attribution/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						OR a.LogData.value('(/ProjectAttachment/NEW_VALUES/OLD_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						OR a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@ProjectId)[1]', 'int') = @ProjectId
							)
					AND (@PersonId IS NULL 
						OR a.PersonId = @PersonId
						OR a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId 
						OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectPersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectName)[1]', 'NVARCHAR(85)') = @PersonLastFirstName
						OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
						OR (a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId AND a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 3)
						OR a.LogData.value('(/Export/NEW_VALUES/@User)[1]','NVARCHAR(85)') = @PersonLastFirstName
						OR (a.LogData.value('(/Attribution/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId AND a.LogData.value('(/Attribution/NEW_VALUES/@AttributionRecordTypeId)[1]', 'int') = 1) --Person Type
						OR (a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@TargetId)[1]', 'int') = @PersonId AND a.LogData.value('(/Attribution/NEW_VALUES/OLD_VALUES/@AttributionRecordTypeId)[1]', 'int') = 1) --Person Type
						OR (a.LogData.value('(/PersonCalendar/NEW_VALUES/@PersonId)[1]', 'int') = @PersonId ) 
						OR (a.LogData.value('(/PersonCalendar/NEW_VALUES/OLD_VALUES/@PersonId)[1]', 'int') = @PersonId )
						OR (a.LogData.value('(/SubstituteHoliday/NEW_VALUES/@PersonId)[1]', 'int') = @PersonId ) 
						OR (a.LogData.value('(/SubstituteHoliday/NEW_VALUES/OLD_VALUES/@PersonId)[1]', 'int') = @PersonId )
						)

