CREATE PROCEDURE [dbo].[InsertProjectFeedbackByMilestonePersonId]
(
	@MilestonePersonId   INT=NULL,
	@MilestoneId	INT=NULL
)
AS
BEGIN


DECLARE @TestTable TABLE(MilestonePersonId int,Date Datetime, RowNumber int,primary key(RowNumber, MilestonePersonId))

DELETE PF
FROM dbo.ProjectFeedback PF
INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = PF.MilestonePersonId
INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
WHERE (@MilestonePersonId IS NULL OR MP.MilestonePersonId = @MilestonePersonId)
	  AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	  AND PF.FeedbackStatusId <> 1

;WITH ReArrangedMilestoneRecords 
	AS
	(
		SELECT ROW_NUMBER() OVER(ORDER BY MilestonePersonId) RNo,MilestonePersonId,StartDate,EndDate
		FROM MilestonePersonEntry
		UNION ALL
		SELECT A.RNo,A.MilestonePersonId,B.StartDate,B.EndDate
		FROM ReArrangedMilestoneRecords  A
		JOIN MilestonePersonEntry B
		ON A.MilestonePersonId = B.MilestonePersonId
		AND A.EndDate +1 = B.StartDate
	),
	MilestoneRecordsContinuity
	as
	(
	SELECT P.MilestonePersonId, P.StartDate,P.EndDate
	FROM
	(
		SELECT A.RNo,A.MilestonePersonId,MIN(A.StartDate) StartDate,MAX(A.EndDate) EndDate
		FROM ReArrangedMilestoneRecords A
		GROUP BY A.RNo,A.MilestonePersonId
	) P
	LEFT JOIN ReArrangedMilestoneRecords B ON P.MilestonePersonId = B.MilestonePersonId AND P.StartDate BETWEEN B.StartDate AND B.EndDate AND P.RNo <> B.RNo
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = P.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	INNER JOIN dbo.Title T ON T.TitleId = Per.TitleId
	INNER JOIN dbo.GetCurrentPayTypeTable() GC ON GC.PersonId = Per.PersonId 
	WHERE B.MilestonePersonId IS NULL 
	AND DATEDIFF(WEEK,P.StartDate,P.EndDate) >= 6
	AND (@MilestonePersonId IS NULL OR P.MilestonePersonId = @MilestonePersonId)
	AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	AND Per.PersonStatusId IN (1,5) AND Per.IsStrawman = 0
	AND T.Title NOT IN ('Senior Director','Client Director','Practice Director')
	AND GC.Timescale = 2
	AND M.ProjectId <> 174
	)
	,
	AfterRemovingFeedbackRecords
	as
	(
	SELECT  m.MilestonePersonId,c.Date
	FROM MilestoneRecordsContinuity m
	JOIN Calendar c on c.Date BETWEEN m.StartDate AND m.EndDate
	LEFT JOIN ProjectFeedback pf on pf.MilestonePersonId = m.MilestonePersonId AND c.Date BETWEEN pf.ReviewPeriodStartDate AND pf.ReviewPeriodEndDate AND pf.FeedbackStatusId = 1
	WHERE pf.ReviewPeriodStartDate IS NULL
	)
	
	INSERT INTO @TestTable
	SELECT MilestonePersonId,ar.date,ROW_NUMBER() OVER(partition by AR.MilestonePersonId ORDER BY ar.date)
	FROM AfterRemovingFeedbackRecords AR
	
	;WITH FeedbackFreeReArrangedMilestoneRecords
	AS
	(
				SELECT  pc.*,DATEADD([day],
					(-1 * 
					( DENSE_RANK() OVER(PARTITION BY PC.MilestonePersonId ORDER BY pc.date) /* Dense Rank */ )) ,pc.date) as GroupedDate
			FROM  @TestTable AS pc
	),
	FinalRanges
	AS
	(
		SELECT MilestonePersonId,MIN(Date) StartDate,MAX(Date) EndDate
		FROM FeedbackFreeReArrangedMilestoneRecords
		GROUP BY MilestonePersonId,GroupedDate
	)
	INSERT INTO dbo.ProjectFeedback(ProjectId,PersonId,ReviewPeriodStartDate,ReviewPeriodEndDate,DueDate,FeedbackStatusId,IsCanceled,CompletionCertificateBy,CompletionCertificateDate,CancelationReason,MilestonePersonId,NextIntialMailSendDate)
	SELECT M.ProjectId,MP.PersonId,C.[Date],CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN P.EndDate ELSE DATEADD(MONTH,3,C.Date)-1 END,CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN DATEADD(WEEK,2,P.EndDate) ELSE DATEADD(WEEK,2,DATEADD(MONTH,3,C.Date)-1) END,
	       2,0,NULL,NULL,NULL,P.MilestonePersonId,  CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN P.EndDate ELSE DATEADD(MONTH,3,C.Date)-1 END
	FROM FinalRanges p
	JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = p.MilestonePersonId
	JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	JOIN Calendar C ON (DATEDIFF(MONTH, P.StartDate, C.[Date]) % 3 = 0 ) AND DATEPART(DD,P.StartDate) = DATEPART(DD,C.[Date]) and C.[Date] BETWEEN P.StartDate AND P.EndDate
	ORDER BY MilestonePersonId
    OPTION (MAXRECURSION 2500);

END

