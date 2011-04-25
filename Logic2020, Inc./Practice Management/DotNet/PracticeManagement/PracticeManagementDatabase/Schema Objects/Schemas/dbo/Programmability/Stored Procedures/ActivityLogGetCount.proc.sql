CREATE PROCEDURE [dbo].[ActivityLogGetCount]
(
	@StartDate     DATETIME,
	@EndDate       DATETIME,
	@PersonId      INT,
	@ProjectId	   INT = NULL,
	@EventSource   NVARCHAR(20),
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
	5	BillingInfo
	*/

	SELECT COUNT(*) AS NUM
	  FROM dbo.UserActivityLog AS a
	       INNER JOIN dbo.UserActivityType AS t ON a.ActivityTypeID = t.ActivityTypeID
    WHERE a.LogDate BETWEEN @StartDate AND @EndDate
				  AND(
				  ((@EventSource = 'Error' OR @EventSource = 'All' )AND a.LogData.exist('/Error') = 1)								  
				  OR ((@EventSource = 'TimeEntry' OR @EventSource = 'All' ) AND a.LogData.exist('/TimeEntry') = 1)
				  OR (@EventSource = 'All' AND a.LogData.exist('/') = 1)
			      OR ((@EventSource = 'Person' OR @EventSource = 'All')
							 AND (a.LogData.exist('/Person') = 1 
								  OR a.LogData.exist('/Roles') = 1 
								  OR a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 3
								 )
							AND 
								(
									(a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId 
									OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectPersonId)[1]', 'int') = @PersonId
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
								 OR a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') in (1, 2)
							     OR a.LogData.exist('/Milestone') = 1 )
						)
						AND 
						(@ProjectId IS NULL 
						 OR a.LogData.value('(/Milestone/NEW_VALUES/@MilestoneProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Project/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @ProjectId
						 )
					 )
					 
				  OR (
						(@EventSource = 'Opportunity' OR @EventSource = 'All') 
							AND (a.LogData.exist('/Opportunity') = 1 
								OR a.LogData.exist('/OpportunityTransition') = 1 
								OR a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 4
								)
							AND (@OpportunityId IS NULL OR 
								a.LogData.value('(/Opportunity/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId OR 
								a.LogData.value('(/OpportunityTransition/NEW_VALUES/@OpportunityId)[1]', 'int') = @OpportunityId OR
								a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @OpportunityId)
					 ) 
				  OR ((@EventSource = 'Milestone' OR @EventSource = 'All')  
					  AND (a.LogData.exist('/Milestone') = 1 
							OR a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 1
							)
					  AND(@MilestoneId IS NULL 
					     OR a.LogData.value('(/Milestone/NEW_VALUES/@MilestoneId)[1]', 'int') = @MilestoneId 
					     OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @MilestoneId))
																		  
				  OR ((@EventSource = 'ProjectAndMilestones' OR @EventSource = 'All') AND (a.LogData.exist('/Project') = 1 
																	OR a.LogData.exist('/Milestone') = 1
																 ) 
					 )
				  OR ((@EventSource = 'TargetPerson' OR @EventSource = 'All')	 AND ( @PersonId IS NULL 
															OR a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
															OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
															OR a.PersonID = @PersonId
															OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId
														  )
					 ))
	
					AND (@ProjectId IS NULL 
						 OR a.LogData.value('(/Project/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Milestone/NEW_VALUES/@MilestoneProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/MilestonePerson/NEW_VALUES/@MilestoneProjectId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/Note/NEW_VALUES/@ParentTargetId)[1]', 'int') = @ProjectId
						 OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ProjectId)[1]', 'int') = @ProjectId
							)
					AND (@PersonId IS NULL 
						OR a.PersonId = @PersonId
						OR a.LogData.value('(/*/*/@PersonId)[1]', 'int') = @PersonId 
						OR a.LogData.value('(/TimeEntry/NEW_VALUES/@ObjectPersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/*/@PersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@MilestonePersonId)[1]', 'int') = @PersonId
						OR a.LogData.value('(/*/*/@PracticeManagerId)[1]', 'int') = @PersonId
						OR (a.LogData.value('(/Note/NEW_VALUES/@TargetId)[1]', 'int') = @PersonId
							AND a.LogData.value('(/Note/NEW_VALUES/@NoteTargetId)[1]', 'int') = 3)
						)
	       
