CREATE PROCEDURE [dbo].[GetOpportunityPriorityTransitionCount]
(
	@DaysPrevious int
)
AS
BEGIN
	;With OpportunityTransitionPriorityList AS
	(SELECT [OpportunityTransitionId]
			  ,[OpportunityId]
			  ,[OpportunityTransitionStatusId]
			  ,[TransitionDate]
			  ,[PersonId]
			  ,[NoteText]
			  ,[OpportunityTransitionTypeId]
			  ,[TargetPersonId]
			  ,SUBSTRING(REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''),0,
			  CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''))) Previous
				  ,SUBSTRING( REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''),
								CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''))+1
								,LEN(REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''))-
								CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Priority changed.  Was: ',''),'now',''),' ',''))
			  ) next
		  FROM OpportunityTransition
		  WHERE [OpportunityTransitionStatusId] = 2
		  AND TransitionDate >= dbo.GettingPMTime(GETUTCDATE()) - @DaysPrevious --Get last @days days record.
	),

	PriorityListDerived AS
	(
	SELECT opl.OpportunityId,
			opl.TransitionDate,
			opl.Previous,
			opl.next,
			MAX(opl.TransitionDate) OVER(partition by opl.OpportunityId) AS 'MaxDate',
			MIN(opl.TransitionDate) OVER(partition by opl.OpportunityId) AS 'MinDate',
			case WHEN opl.TransitionDate = MAX(opl.TransitionDate) OVER(partition by opl.OpportunityId) THEN np.sortOrder ELSE NULL END [NextSortOrder],
			case WHEN opl.TransitionDate = MIN(opl.TransitionDate) OVER(partition by opl.OpportunityId) THEN pp.sortOrder ELSE NULL END [PreviousSortOrder]
	FROM OpportunityTransitionPriorityList opl
	INNER JOIN OpportunityPriorities np On np.Priority = opl.next
	INNER JOIN OpportunityPriorities pp On pp.Priority = opl.Previous
	WHERE NoteText LIKE '%Priority changed.%'

	)

	SELECT (case WHEN priorityTrend.Status = 1 THEN 'Up'
				WHEN priorityTrend.Status = 0 THEN 'Down'
				ELSE 'Equal' END) [PriorityTrendType], COUNT(priorityTrend.OpportunityId) PriorityTrendCount
	FROM (
		SELECT pld.OpportunityId, (case WHEN SUM(pld.NextSortOrder) < SUM(pld.PreviousSortOrder) THEN 1
													WHEN SUM(pld.NextSortOrder) > SUM(pld.PreviousSortOrder) THEN 0
													ELSE null END) AS [Status]
		FROM PriorityListDerived pld
		GROUP BY pld.OpportunityId
		) AS priorityTrend
	WHERE priorityTrend.Status IS NOT NULL --To read only up and down.
	GROUP BY priorityTrend.Status
	ORDER BY priorityTrend.Status DESC

END
