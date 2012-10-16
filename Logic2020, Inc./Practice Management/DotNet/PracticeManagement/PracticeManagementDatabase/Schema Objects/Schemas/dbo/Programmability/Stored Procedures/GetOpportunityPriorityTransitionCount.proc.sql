CREATE PROCEDURE [dbo].[GetOpportunityPriorityTransitionCount]
(
	@DaysPrevious int
)
AS
BEGIN
	;With OpportunityTransitionPriorityList AS
	(SELECT [OpportunityTransitionId]
			  ,ot.[OpportunityId]
			  ,[OpportunityTransitionStatusId]
			  ,[TransitionDate]
			  ,[PersonId]
			  ,[NoteText]
			  ,[OpportunityTransitionTypeId]
			  ,[TargetPersonId]
			  ,PreviousChangedId
			  ,NextChangedId
		  FROM OpportunityTransition ot
		  JOIN Opportunity O On O.OpportunityId = ot.OpportunityId
		  WHERE [OpportunityTransitionStatusId] = 2 AND O.OpportunityStatusId = 1 AND NoteText LIKE 'Sales Stage changed.%'
		  AND TransitionDate >= dbo.GettingPMTime(GETUTCDATE()) - @DaysPrevious --Get last @days days record.
	),

	PriorityListDerived AS
	(
	SELECT opl.OpportunityId,
			opl.TransitionDate,
			opl.PreviousChangedId,
			opl.NextChangedId,
			MAX(opl.TransitionDate) OVER(partition by opl.OpportunityId) AS 'MaxDate',
			MIN(opl.TransitionDate) OVER(partition by opl.OpportunityId) AS 'MinDate',
			case WHEN opl.TransitionDate = MAX(opl.TransitionDate) OVER(partition by opl.OpportunityId) THEN np.sortOrder ELSE NULL END [NextSortOrder],
			case WHEN opl.TransitionDate = MIN(opl.TransitionDate) OVER(partition by opl.OpportunityId) THEN pp.sortOrder ELSE NULL END [PreviousSortOrder]
	FROM OpportunityTransitionPriorityList opl
	INNER JOIN OpportunityPriorities np On np.Id = opl.NextChangedId
	INNER JOIN OpportunityPriorities pp On pp.Id = opl.PreviousChangedId
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

