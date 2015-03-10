CREATE PROCEDURE [dbo].[UpdateMSBadgeDetailsByPersonId]
(
	@PersonId		INT = NULL,
	@UpdatedBy		INT,
	@MilestoneId	INT = NULL
)
AS
BEGIN

	DECLARE @DefaultStartDate DATETIME = '20140701'
	DECLARE @BadgeTable1 TABLE(
								PersonId INT,
								BadgeStartDate DATETIME,
								BadgeEndDate DATETIME,
								BadgeSource	NVARCHAR(30),
								GapInMonths INT,
								RNum	INT
							 )

    DECLARE @BadgeTable2 TABLE(
								PersonId INT,
								StartDate DATETIME,
								PlannedEnd DATETIME,
								BadgeStartSource NVARCHAR(30),
								PlannedEndSource NVARCHAR(30)
							 )

    DECLARE @BadgeTable3 TABLE(
								PersonId INT,
								StartDate DATETIME,
								PlannedEnd DATETIME
							 )

	SET ANSI_WARNINGS OFF;
	;WITH BadgeDetails
	AS
	(
		SELECT B.PersonId,B.BadgeStartDate,B.BadgeEndDate,B.ProjectNumber,B.IsBadgeException,ROW_NUMBER() OVER(PARTITION BY B.PersonId ORDER BY B.BadgeStartDate) AS RNo
		FROM
		(
			SELECT MP.PersonId,MPE.BadgeStartDate,MPE.BadgeEndDate,MPE.IsBadgeException,P.ProjectNumber
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE (@PersonId IS NULL OR MP.PersonId = @PersonId)
					AND
				  (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
			AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 1 AND P.ProjectStatusId IN (1,2,3,4) --Inactive,Completed,active and projected statuses.
		
			UNION ALL

			SELECT M.PersonId, M.LastBadgeStartDate,M.LastBadgeEndDate,0,'Previous Badge History'
			FROM dbo.MSBadge M 
			WHERE M.PersonId = @PersonId AND M.IsPreviousBadge = 1
		) B
	),
	BadgeDetails2
	AS
	(
		SELECT B1.PersonId,B1.BadgeStartDate,B1.BadgeEndDate,B1.ProjectNumber,DATEDIFF(MM,B1.BadgeEndDate,b2.BadgeStartDate) AS GapInMonths, ROW_NUMBER() OVER(PARTITION BY B1.PersonId ORDER BY B1.BadgeStartDate) AS RNum
		FROM BadgeDetails B1 
		LEFT JOIN BadgeDetails B2 ON B1.PersonId = B2.PersonId AND B2.RNo = B1.RNo+1
	)
	INSERT INTO @BadgeTable1
	SELECT * FROM BadgeDetails2

	INSERT INTO @BadgeTable2
	SELECT B.PersonId,b.BadgeStartDate AS StartDate,b.BadgeEndDate as PlannedEnd,B.BadgeSource,B.BadgeSource
	FROM @BadgeTable1 B 
	INNER JOIN (
					SELECT PersonId,max(Rnum)+1 AS Rnum
					FROM @BadgeTable1
					WHERE GapInMonths >= 6
					GROUP BY PersonId ) B1 ON B.PersonId = B1.PersonId AND B1.Rnum = B.RNum
	INNER JOIN (SELECT PersonId,ISNULL(MAX(GapInMonths),0) GAP FROM @BadgeTable1 GROUP BY PersonId) B2 ON B2.PersonId = B.PersonId AND B2.GAP >= 6

	INSERT INTO @BadgeTable2
	SELECT B.PersonId,MIN(b.BadgeStartDate) Start,MAX(b.BadgeEndDate),(SELECT TOP 1 BT.BadgeSource FROM @BadgeTable1 BT WHERE BT.BadgeStartDate = MIN(b.BadgeStartDate)),
		   (SELECT TOP 1 BT1.BadgeSource FROM @BadgeTable1 BT1 WHERE BT1.BadgeEndDate = MAX(b.BadgeEndDate))  
	FROM @BadgeTable1 B 
	INNER JOIN (SELECT PersonId,ISNULL(MAX(GapInMonths),0) GAP FROM @BadgeTable1 GROUP BY PersonId) B1 ON B1.PersonId = B.PersonId AND B1.GAP < 6
	GROUP BY B.PersonId

	
	UPDATE M
	SET BadgeStartDate =   CASE	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN B.StartDate 
								ELSE @DefaultStartDate END,
		BadgeEndDate =	 CASE	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate)-1 ELSE B.PlannedEnd END) 
								ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate)-1 ELSE B.PlannedEnd END) END,
		PlannedEndDate = B.PlannedEnd,
		BadgeStartDateSource = CASE	WHEN B.StartDate IS NULL THEN NULL
									ELSE B.BadgeStartSource END, -- BADGE HISTORY
		BadgeEndDateSource = CASE WHEN B.StartDate IS NULL THEN NULL
									ELSE B.BadgeStartSource END, -- BADGE HISTORY
		PlannedEndDateSource = CASE	WHEN B.StartDate IS NULL THEN NULL
									ELSE B.PlannedEndSource END, -- BADGE HISTORY
		BreakStartDate = CASE 	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate) ELSE B.PlannedEnd+1 END)-- DATEADD(MM,18,B.StartDate) 
								ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate) ELSE B.PlannedEnd+1 END) END,
		BreakEndDate =	CASE	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,B.StartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) --DATEADD(MM,24,B.StartDate)-1 
								ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,@DefaultStartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) END
	FROM MSBadge M
	LEFT JOIN @BadgeTable2 B ON B.PersonId = M.PersonId
	WHERE (@PersonId IS NULL OR m.PersonId = @PersonId)
	AND (@MilestoneId IS NULL OR B.PersonId IS NOT NULL)

    ----Merge Project Override Exception dates
    UPDATE M
	SET BadgeStartDate = CASE WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NULL THEN NULL
							  WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NOT NULL THEN ExceptionStartDate
							  WHEN BadgeStartDate IS NOT NULL AND ExceptionStartDate IS NULL THEN BadgeStartDate
							  WHEN BadgeStartDate < ExceptionStartDate THEN BadgeStartDate 
							  ELSE ExceptionStartDate END,
	    BadgeEndDate =  CASE WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NULL THEN NULL
							  WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NOT NULL THEN (CASE WHEN DATEADD(MM,18,ExceptionStartDate)-1 > ExceptionEndDate THEN DATEADD(MM,18,ExceptionStartDate)-1 ELSE ExceptionEndDate END)
							  WHEN BadgeEndDate IS NOT NULL AND ExceptionEndDate IS NULL THEN BadgeEndDate
							  WHEN BadgeEndDate < ExceptionEndDate THEN ExceptionEndDate 
							  ELSE BadgeEndDate END,
	   PlannedEndDate = CASE WHEN M.PlannedEndDate IS NULL AND ExceptionEndDate IS NULL THEN NULL
							 WHEN M.PlannedEndDate IS NULL AND ExceptionEndDate IS NOT NULL THEN (CASE WHEN DATEADD(MM,18,ExceptionStartDate)-1 > ExceptionEndDate THEN DATEADD(MM,18,ExceptionStartDate)-1 ELSE ExceptionEndDate END)
							 ELSE M.PlannedEndDate END,
	   BadgeStartDateSource = CASE WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NULL THEN NULL
							  WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NOT NULL THEN 'MS Exception'
							  WHEN BadgeStartDate IS NOT NULL AND ExceptionStartDate IS NULL THEN B.BadgeStartSource
							  WHEN BadgeStartDate < ExceptionStartDate THEN B.BadgeStartSource
							  ELSE 'MS Exception' END,
	   PlannedEndDateSource = CASE WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NULL THEN NULL
							  WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NOT NULL THEN 'MS Exception'
							  WHEN BadgeStartDate IS NOT NULL THEN B.PlannedEndSource END,

	   BadgeEndDateSource =	  CASE WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NULL THEN NULL
							  WHEN BadgeStartDate IS NULL AND ExceptionStartDate IS NOT NULL THEN 'MS Exception'
							  WHEN BadgeStartDate IS NOT NULL AND ExceptionStartDate IS NULL THEN B.BadgeStartSource
							  WHEN ExceptionEndDate < BadgeEndDate THEN B.BadgeStartSource
							  ELSE 'MS Exception' END,

	   BreakStartDate = CASE WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NULL THEN NULL
							  WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NOT NULL THEN (CASE WHEN DATEADD(MM,18,ExceptionStartDate)-1 > ExceptionEndDate THEN DATEADD(MM,18,ExceptionStartDate) ELSE ExceptionEndDate+1 END)
							  WHEN BadgeEndDate IS NOT NULL AND ExceptionEndDate IS NULL THEN BreakStartDate
							  WHEN BadgeEndDate < ExceptionEndDate THEN ExceptionEndDate+1 
							  ELSE BreakStartDate END,
	   BreakEndDate = CASE WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NULL THEN NULL
							  WHEN BadgeEndDate IS NULL AND ExceptionEndDate IS NOT NULL THEN DATEADD(mm,6,(CASE WHEN DATEADD(MM,18,ExceptionStartDate)-1 > ExceptionEndDate THEN DATEADD(MM,18,ExceptionStartDate)-1 ELSE ExceptionEndDate END))
							  WHEN BadgeEndDate IS NOT NULL AND ExceptionEndDate IS NULL THEN BreakEndDate
							  WHEN BadgeEndDate < ExceptionEndDate THEN DATEADD(mm,6,ExceptionEndDate)
							  ELSE BreakEndDate END
	FROM MSBadge M
	LEFT JOIN @BadgeTable2 B ON B.PersonId = M.PersonId
	WHERE (@PersonId IS NULL OR m.PersonId = @PersonId)
	AND (@MilestoneId IS NULL OR B.PersonId IS NOT NULL)

	EXEC [dbo].[SavePersonBadgeHistories] @PersonId	= @PersonId, @UpdatedBy = @UpdatedBy
	-------------------------------------------------------
	----- Insert into BadgeHistoryForReports table --------
	-------------------------------------------------------
	INSERT INTO @BadgeTable3
    SELECT B.PersonId,b.BadgeStartDate AS StartDate,b.BadgeEndDate as PlannedEnd
	FROM @BadgeTable1 B 
	INNER JOIN (
					SELECT PersonId,min(Rnum) AS Rnum
					FROM @BadgeTable1
					WHERE GapInMonths >= 6
					GROUP BY PersonId ) B1 ON B.PersonId = B1.PersonId AND B1.Rnum = B.RNum
	INNER JOIN (SELECT PersonId,ISNULL(MAX(GapInMonths),0) GAP FROM @BadgeTable1 GROUP BY PersonId) B2 ON B2.PersonId = B.PersonId AND B2.GAP >= 6
	
	INSERT INTO @BadgeTable3
	SELECT B.PersonId,MIN(b.BadgeStartDate) Start,MAX(b.BadgeEndDate) 
	FROM @BadgeTable1 B 
	INNER JOIN (SELECT PersonId,ISNULL(MAX(GapInMonths),0) GAP FROM @BadgeTable1 GROUP BY PersonId) B1 ON B1.PersonId = B.PersonId AND B1.GAP < 6
	GROUP BY B.PersonId

	--Delete person previous history records
	DELETE BadgeHistoryForReports
	WHERE PersonId = @PersonId

	--REINSERT WITH NEW VALUES
	INSERT INTO BadgeHistoryForReports(PersonId,BadgeStartDate,BadgeEndDate,ProjectPlannedEndDate,BreakStartDate,BreakEndDate)
	SELECT B.PersonId, CASE	WHEN B.StartDate > @DefaultStartDate THEN B.StartDate 
							ELSE @DefaultStartDate END,
					   CASE WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate)-1 ELSE B.PlannedEnd END) 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate)-1 ELSE B.PlannedEnd END) END,
					   B.PlannedEnd,
					   CASE WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate) ELSE B.PlannedEnd+1 END)-- DATEADD(MM,18,B.StartDate) 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate) ELSE B.PlannedEnd+1 END) END,
					   CASE	WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,B.StartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) --DATEADD(MM,24,B.StartDate)-1 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,@DefaultStartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) END
	FROM @BadgeTable3 B

	IF NOT EXISTS(SELECT 1 FROM BadgeHistoryForReports WHERE PersonId = @PersonId)
	BEGIN
	  INSERT INTO BadgeHistoryForReports(PersonId,BadgeStartDate,BadgeEndDate,ProjectPlannedEndDate,BreakStartDate,BreakEndDate)
	  SELECT PersonId,BadgeStartDate,BadgeEndDate,PlannedEndDate,BreakStartDate,BreakEndDate
	  FROM MSBadge 
	  WHERE PersonId = @PersonId
	END

END
