CREATE PROCEDURE [dbo].[GetOpportunityStatusChangeCount]
(
	@DaysPrevious INT
)
AS
BEGIN

	;With OpportunityStatusList AS
	(SELECT [OpportunityTransitionId]
			  ,[OpportunityId]
			  ,[OpportunityTransitionStatusId]
			  ,[TransitionDate]
			  ,[PersonId]
			  ,[NoteText]
			  ,[OpportunityTransitionTypeId]
			  ,[TargetPersonId]
			  ,SUBSTRING(REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''),0,
			  CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''))) Previous
				  ,SUBSTRING( REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''),
								CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''))+1
								,LEN(REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''))-
								CHARINDEX(':',REPLACE(REPLACE(REPLACE([NoteText],'Status changed.  Was: ',''),'now',''),' ',''))
			  ) [Next]
		  FROM OpportunityTransition
		  WHERE [OpportunityTransitionStatusId] = 2
		  AND TransitionDate >= (dbo.GettingPMTime(getUTCDATE()) - @DaysPrevious) --get last @days days record.
		  AND NoteText LIKE '%Status changed.%'
	),
	
	
	StatusListWithPreviousAndCurrentStatus AS
	(
		SELECT osl.OpportunityId,
				osl.TransitionDate,
				osl.Previous,
				osl.[Next],
				(case WHEN osl.TransitionDate = MAX(osl.TransitionDate) OVER(partition by osl.OpportunityId) THEN osl.[Next] ELSE NULL END) AS [CurrentStatus],
				(case WHEN osl.TransitionDate = MIN(osl.TransitionDate) OVER(partition by osl.OpportunityId) THEN osl.[Previous] ELSE NULL END) AS [PreviousStatus]
		FROM OpportunityStatusList osl
	),
	
	StatusListOnlyDifferentFromPreviousToCurrent AS
	(
		SELECT sld.OpportunityId,
				MAX(CurrentStatus) currentStatus
		FROM StatusListWithPreviousAndCurrentStatus sld
		GROUP BY sld.OpportunityId
		HAVING MAX(CurrentStatus) <> MAX(PreviousStatus)
	)

	SELECT sld.CurrentStatus AS [Status], COUNT(sld.OpportunityId) as StatusCount
	FROM StatusListOnlyDifferentFromPreviousToCurrent as sld
	WHERE sld.CurrentStatus IS NOT NULL
	GROUP BY CurrentStatus

END
