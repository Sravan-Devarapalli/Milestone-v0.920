CREATE PROCEDURE [dbo].[InsertFeedbackByWeekWise]
(
	@MilestonePersonId   INT=NULL,
	@MilestoneId		 INT=NULL,
	@ProjectId			 INT=NULL
)
AS
BEGIN

	DECLARE @TestTable TABLE(PersonId INT,ProjectId INT, Date DATETIME, RowNumber INT,PRIMARY KEY(RowNumber, PersonId, ProjectId))
	
	DECLARE @ProjectIdLocal INT, @PersonId INT
	
	SELECT @ProjectIdLocal = M.ProjectId 
	FROM Milestone M
	JOIN MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	WHERE (@MilestonePersonId IS NULL OR MP.MilestonePersonId = @MilestonePersonId)
		  AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)

    SELECT @PersonId = PersonId FROM MilestonePerson WHERE MilestonePersonId = @MilestonePersonId

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
		SELECT MP.PersonId, M.ProjectId, P.StartDate,P.EndDate
		FROM
		(
			SELECT A.RNo,A.MilestonePersonId,MIN(A.StartDate) StartDate,MAX(A.EndDate) EndDate
			FROM ReArrangedMilestoneRecords A
			GROUP BY A.RNo,A.MilestonePersonId
		) P
		LEFT JOIN ReArrangedMilestoneRecords B ON P.MilestonePersonId = B.MilestonePersonId AND P.StartDate BETWEEN B.StartDate AND B.EndDate AND P.RNo <> B.RNo
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = P.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project Pr ON Pr.ProjectId = M.ProjectId
		INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
		INNER JOIN dbo.Title T ON T.TitleId = Per.TitleId
		INNER JOIN dbo.GetCurrentPayTypeTable() GC ON GC.PersonId = Per.PersonId 
		WHERE B.MilestonePersonId IS NULL 
		AND DATEDIFF(WEEK,[dbo].[SmallerDateBetweenTwo](P.StartDate,'20141231'), [dbo].[SmallerDateBetweenTwo](P.EndDate,'20141231')) >= 6
		AND (@MilestonePersonId IS NULL OR (MP.PersonId = @PersonId and M.ProjectId = @ProjectIdLocal))
		AND (@MilestoneId IS NULL OR M.ProjectId = @ProjectIdLocal)
		AND (@ProjectId IS NULL OR Pr.ProjectId = @ProjectId)
		AND Per.PersonStatusId IN (1,5) AND Per.IsStrawman = 0
		AND T.Title NOT IN ('Senior Director','Client Director','Practice Director')
		AND GC.Timescale = 2
		AND M.ProjectId <> 174
		),
		AfterRemovingFeedbackRecords
		as
		(
		SELECT  m.PersonId,m.ProjectId,c.Date
		FROM MilestoneRecordsContinuity m
		JOIN Calendar c on c.Date BETWEEN m.StartDate AND m.EndDate
		LEFT JOIN ProjectFeedback pf on pf.PersonId = m.PersonId AND pf.ProjectId = m.ProjectId AND c.Date BETWEEN pf.ReviewPeriodStartDate AND pf.ReviewPeriodEndDate AND (PF.FeedbackStatusId = 1 OR PF.IsCanceled = 1)
		WHERE pf.ReviewPeriodStartDate IS NULL
		AND C.Date <= '20141231'
		)

		INSERT INTO @TestTable
		SELECT AR.PersonId,AR.ProjectId,ar.date,ROW_NUMBER() OVER(partition by AR.PersonId,AR.ProjectId ORDER BY ar.date)
		FROM AfterRemovingFeedbackRecords AR
	
		;WITH FeedbackFreeReArrangedMilestoneRecords
		AS
		(
					SELECT  pc.*,DATEADD([day],
						(-1 * 
						( DENSE_RANK() OVER(PARTITION BY PC.PersonId,PC.ProjectId ORDER BY pc.date) /* Dense Rank */ )) ,pc.date) as GroupedDate
				FROM  @TestTable AS pc
		),
		FinalRanges
		AS
		(
			SELECT PersonId,ProjectId,MIN(Date) StartDate,MAX(Date) EndDate
			FROM FeedbackFreeReArrangedMilestoneRecords
			GROUP BY PersonId,ProjectId,GroupedDate
		)
		INSERT INTO dbo.ProjectFeedback(ProjectId,PersonId,ReviewPeriodStartDate,ReviewPeriodEndDate,DueDate,FeedbackStatusId,IsCanceled,CompletionCertificateBy,CompletionCertificateDate,CancelationReason,NextIntialMailSendDate,IsGap)
		SELECT p.ProjectId,P.PersonId,C.[Date],CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN P.EndDate ELSE DATEADD(MONTH,3,C.Date)-1 END,CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN DATEADD(WEEK,2,P.EndDate) ELSE DATEADD(WEEK,2,DATEADD(MONTH,3,C.Date)-1) END,
			   2,0,NULL,NULL,NULL, CASE WHEN DATEADD(MONTH,3,C.Date) > P.EndDate THEN P.EndDate ELSE DATEADD(MONTH,3,C.Date)-1 END,0
		FROM FinalRanges p
		JOIN Calendar C ON (DATEDIFF(MONTH, P.StartDate, C.[Date]) % 3 = 0 ) AND DATEPART(DD,P.StartDate) = DATEPART(DD,C.[Date]) and C.[Date] BETWEEN P.StartDate AND P.EndDate
		ORDER BY PersonId
		OPTION (MAXRECURSION 2500);

END
