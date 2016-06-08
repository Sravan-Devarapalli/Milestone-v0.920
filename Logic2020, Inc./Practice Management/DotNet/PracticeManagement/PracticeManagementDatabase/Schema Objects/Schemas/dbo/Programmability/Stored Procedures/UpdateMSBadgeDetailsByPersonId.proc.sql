CREATE PROCEDURE [dbo].[UpdateMSBadgeDetailsByPersonId]
(
	@PersonId		INT = NULL,
	@UpdatedBy		INT,
	@MilestoneId	INT = NULL
)
AS
BEGIN

	EXEC [dbo].[ClearBadgeDeactivationDates] @PersonId = @PersonId

	IF @PersonId IS NOT NULL
	BEGIN
		EXEC dbo.SaveBadgeBreakHistory @PersonId = @PersonId,@UpdatedBy=@UpdatedBy
	END

	DECLARE @FirstDeactivationDate DATETIME=NULL,
			@LastOrganiBreakEnd DATETIME =NULL

	SELECT @FirstDeactivationDate = MIN(d.DeactivatedDate), @LastOrganiBreakEnd =MAX(d.OrganicBreakEndDate)
	FROM BadgeDeactivationHisroty d
	WHERE (@PersonId IS NULL OR d.PersonId=@PersonId)

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
								PlannedEnd DATETIME,
								BadgeStartDateSource NVARCHAR(30) NULL,
								PlannedEndDateSource NVARCHAR(30) NULL
							 )

    DECLARE @BCTE TABLE(
							PersonId INT,
							BadgeStartDate DATETIME,
							BadgeEndDate DATETIME,
							BadgeSource NVARCHAR(30),
							RNum INT,
							Rn INT
						)

	DECLARE @ProjectAfterOrganicBreakEnd TABLE(
							PersonId INT,
							BadgeStartDate DATETIME,
							BadgeEndDate DATETIME,
							ProjectNumber NVARCHAR(30),
							Rno INT
	)

	 declare @badgeDeactivatedhistory table(PersonId int, DeactivatedDate datetime, OrganicBreakStartDate datetime, OrganicBreakEndDate datetime)
	 insert into @badgeDeactivatedhistory
	 select PersonId, DeactivatedDate, OrganicBreakStartDate, OrganicBreakEndDate
	 from BadgeDeactivationHisroty where personid = @PersonId

	 union all

	 select PersonId, DeactivatedDate, OrganicBreakStartDate, OrganicBreakEndDate
	 from MSBadge where personid = @PersonId and DeactivatedDate is not null
	


	INSERT into @ProjectAfterOrganicBreakEnd
	SELECT PersonId, BadgeStartDate, BadgeEndDate, ProjectNumber, ROW_NUMBER() OVER (PARTITION BY PersonId, deactivateddate order by PersonId,BadgeStartDate) ProjectNumberAfterDeact
	FROM
	(
		SELECT MP.PersonId,MPE.BadgeStartDate,MPE.BadgeEndDate,P.ProjectNumber, bdh.deactivateddate, ROW_NUMBER() over (partition by MP.PersonId,MPE.BadgeStartDate,MPE.BadgeEndDate,P.ProjectNumber order by MP.PersonId,MPE.BadgeStartDate,bdh.deactivateddate desc) RN
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		left join (select * from @badgeDeactivatedhistory b where b.PersonId = @PersonId) bdh on bdh.OrganicBreakEndDate < MPE.BadgeStartDate
		WHERE (@PersonId IS NULL OR MP.PersonId = @PersonId)
				AND
				(@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
		AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 1 AND P.ProjectStatusId IN (1,2,3,4) --Inactive,Completed,active and projected statuses.
		AND (MPE.BadgeStartDate>=@FirstDeactivationDate)
	)S
	where S.RN = 1
	--INSERT into @ProjectAfterOrganicBreakEnd
	--SELECT MP.PersonId,MPE.BadgeStartDate,MPE.BadgeEndDate,P.ProjectNumber,ROW_NUMBER() OVER(PARTITION BY MP.PersonId ORDER BY MPE.BadgeStartDate)
	--		FROM dbo.MilestonePersonEntry MPE
	--		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	--		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	--		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	--		WHERE (@PersonId IS NULL OR MP.PersonId = @PersonId)
	--				AND
	--			  (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	--		AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 1 AND P.ProjectStatusId IN (1,2,3,4) --Inactive,Completed,active and projected statuses.
	--		AND (MPE.BadgeStartDate>=@FirstDeactivationDate)

	SET ANSI_WARNINGS OFF;
	;WITH BadgeDetails
	AS
	(
		SELECT B.PersonId,B.BadgeStartDate,B.BadgeEndDate,B.ProjectNumber,B.IsBadgeException,IsProject,ROW_NUMBER() OVER(PARTITION BY B.PersonId ORDER BY B.BadgeStartDate,B.IsProject) AS RNo
		FROM
		(
			SELECT MP.PersonId,MPE.BadgeStartDate,MPE.BadgeEndDate,MPE.IsBadgeException,P.ProjectNumber,1 AS IsProject
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE (@PersonId IS NULL OR MP.PersonId = @PersonId)
					AND
				  (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
			AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 1 AND P.ProjectStatusId IN (1,2,3,4) --Inactive,Completed,active and projected statuses.
			AND (@FirstDeactivationDate IS NULL OR MPE.BadgeEndDate<@FirstDeactivationDate)

			UNION ALL

			SELECT M.PersonId, M.LastBadgeStartDate,M.LastBadgeEndDate,0,'Previous Badge History',1
			FROM dbo.MSBadge M 
			WHERE M.PersonId = @PersonId AND M.IsPreviousBadge = 1

			UNION ALL

			SELECT M.PersonId, M.ExceptionStartDate, CASE WHEN (SELECT 1 FROM dbo.MSBadge M WHERE M.PersonId = @PersonId AND M.DeactivatedDate IS NOT NULL) = 1 THEN (SELECT DeactivatedDate FROM dbo.MSBadge WHERE PersonId = @PersonId AND DeactivatedDate IS NOT NULL) 
														  ELSE M.ExceptionEndDate END, 1, 'MS Exception',0
			FROM dbo.MSBadge M
			WHERE M.PersonId = @PersonId AND M.IsException = 1

			UNION ALL

			SELECT M.PersonId, M.ManualStartDate, CASE WHEN (SELECT 1 FROM dbo.MSBadge M WHERE M.PersonId = @PersonId AND M.DeactivatedDate IS NOT NULL) = 1 THEN (SELECT DeactivatedDate FROM dbo.MSBadge WHERE PersonId = @PersonId AND DeactivatedDate IS NOT NULL)
													   ELSE M.ManualEndDate END, 0, 'Manual Entry',0
			FROM dbo.MSBadge M
			WHERE M.PersonId = @PersonId AND M.ManualStartDate IS NOT NULL

			UNION ALL

			select P.PersonId, P.BadgeStartDate, P.BadgeEndDate,0,P.ProjectNumber, CASE WHEN p.Rno =1 THEN 0 ELSE 1 END
			FROM @ProjectAfterOrganicBreakEnd P
			
		) B
	),
	BadgeDetails2
	AS
	(
		SELECT B1.PersonId,B1.BadgeStartDate,B1.BadgeEndDate,B1.ProjectNumber,CASE WHEN B2.IsProject = 1 THEN 1 ELSE DATEDIFF(MM,B1.BadgeEndDate,b2.BadgeStartDate) END AS GapInMonths, ROW_NUMBER() OVER(PARTITION BY B1.PersonId ORDER BY B1.BadgeStartDate) AS RNum
		FROM BadgeDetails B1 
		LEFT JOIN BadgeDetails B2 ON B1.PersonId = B2.PersonId AND B2.RNo = B1.RNo+1
	)
	INSERT INTO @BadgeTable1
	SELECT * FROM BadgeDetails2

	INSERT INTO @BadgeTable2
	SELECT B.PersonId,b.BadgeStartDate AS StartDate,CASE WHEN B.BadgeSource = 'MS Exception' THEN (SELECT ExceptionEndDate FROM MSBadge WHERE PersonId = @PersonId) 
														 WHEN B.BadgeSource = 'Manual Entry' THEN (SELECT ManualEndDate FROM MSBadge WHERE PersonId = @PersonId) 
														 ELSE b.BadgeEndDate END as PlannedEnd,B.BadgeSource,B.BadgeSource
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
								ELSE dbo.GreaterDateBetweenTwo(B.StartDate,@DefaultStartDate) END,
		BadgeEndDate =	 CASE	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN dbo.GreaterDateBetweenTwo(DATEADD(MM,18,B.StartDate)-1,B.PlannedEnd) 
								ELSE dbo.GreaterDateBetweenTwo(DATEADD(MM,18,@DefaultStartDate)-1,B.PlannedEnd) END,
		PlannedEndDate = CASE WHEN B.PlannedEndSource = 'Manual Entry' THEN NULL
							  ELSE B.PlannedEnd END,
		BadgeStartDateSource = CASE	WHEN B.StartDate IS NULL THEN NULL
									ELSE B.BadgeStartSource END, -- BADGE HISTORY
		BadgeEndDateSource = CASE WHEN B.StartDate IS NULL THEN NULL
								  WHEN B.PlannedEnd > DATEADD(MM,18,B.StartDate)-1 AND B.PlannedEnd > DATEADD(MM,18,@DefaultStartDate)-1 THEN B.PlannedEndSource
								  ELSE B.BadgeStartSource END,
		PlannedEndDateSource = CASE	WHEN B.StartDate IS NULL THEN NULL
									WHEN B.PlannedEndSource ='Manual Entry' THEN NULL 
									ELSE B.PlannedEndSource  END, -- BADGE HISTORY
		BreakStartDate = CASE 	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN dbo.GreaterDateBetweenTwo(DATEADD(MM,18,B.StartDate),B.PlannedEnd+1) -- DATEADD(MM,18,B.StartDate) 
								ELSE dbo.GreaterDateBetweenTwo(DATEADD(MM,18,@DefaultStartDate),B.PlannedEnd+1) END,
		BreakEndDate =	CASE	WHEN B.StartDate IS NULL THEN NULL
								WHEN B.StartDate > @DefaultStartDate THEN dbo.GreaterDateBetweenTwo(DATEADD(MM,24,B.StartDate)-1,DATEADD(MM,6,B.PlannedEnd)) --DATEADD(MM,24,B.StartDate)-1 
								ELSE dbo.GreaterDateBetweenTwo(DATEADD(MM,24,@DefaultStartDate)-1,DATEADD(MM,6,B.PlannedEnd)) END,
		DeactivatedDate = CASE WHEN B.StartDate IS NULL THEN NULL
							   ELSE M.DeactivatedDate END,
		OrganicBreakStartDate= CASE WHEN B.StartDate IS NULL THEN NULL
							   ELSE M.OrganicBreakStartDate END,
		OrganicBreakEndDate=   CASE WHEN B.StartDate IS NULL THEN NULL
							   ELSE M.OrganicBreakEndDate END

	FROM MSBadge M
	LEFT JOIN @BadgeTable2 B ON B.PersonId = M.PersonId
	WHERE (@PersonId IS NULL OR m.PersonId = @PersonId)
	AND (@MilestoneId IS NULL OR B.PersonId IS NOT NULL) AND (@LastOrganiBreakEnd IS NULL OR B.StartDate > @LastOrganiBreakEnd)


	EXEC [dbo].[SavePersonBadgeHistories] @PersonId	= @PersonId, @UpdatedBy = @UpdatedBy
	-------------------------------------------------------
	----- Insert into BadgeHistoryForReports table --------
	-------------------------------------------------------
	Declare @Count int
	Declare @BCTECount int
	select @Count = MAX(RNum) from @BadgeTable1 group by PersonId

	INSERT INTO @BCTE
	SELECT B.PersonId,B.BadgeStartDate,B.BadgeEndDate,B.BadgeSource,B.RNum,ROW_NUMBER() OVER(PARTITION BY B.PersonId ORDER BY B.BadgeStartDate) Rn
	FROM
	(
		SELECT DISTINCT B.*
		FROM @BadgeTable1 B 
		LEFT JOIN @BadgeTable1 B1 ON B1.RNum in (select RNum+1 from @BadgeTable1 where GapInMonths >= 6)
		WHERE B.RNum = 1 OR B.RNum = B1.RNum 
	) B
	
    SELECT @BCTECount = MAX(Rn) FROM @BCTE GROUP BY PersonId

	INSERT INTO @BadgeTable3(PersonId,StartDate,PlannedEnd,BadgeStartDateSource,PlannedEndDateSource)
	SELECT B.PersonId,B.BadgeStartDate,CASE WHEN ISNULL(ISNULL(B3.BadgeSource,B2.BadgeSource),B.BadgeSource) = 'MS Exception' THEN (SELECT ExceptionEndDate FROM MSBadge WHERE PersonId = @PersonId) 
											WHEN ISNULL(ISNULL(B3.BadgeSource,B2.BadgeSource),B.BadgeSource) = 'Manual Entry' THEN (SELECT ManualEndDate FROM MSBadge WHERE PersonId = @PersonId) 
											ELSE ISNULL(ISNULL(B3.BadgeEndDate,B2.BadgeEndDate),B.BadgeEndDate) END,B.BadgeSource,ISNULL(ISNULL(B3.BadgeSource,B2.BadgeSource),B.BadgeSource)
	FROM @BCTE B 
	LEFT JOIN @BCTE B1 ON B.PersonId = B1.PersonId AND B1.Rn = B.Rn+1 
	LEFT JOIN @BadgeTable1 B2 ON B2.RNum+1 = B1.RNum
	LEFT JOIN @BadgeTable1 B3 ON B3.RNum = @Count AND B.Rn = @BCTECount

	--Delete person previous history records
	DELETE BadgeHistoryForReports
	WHERE PersonId = @PersonId

	--REINSERT WITH NEW VALUES
	INSERT INTO BadgeHistoryForReports(PersonId,BadgeStartDate,BadgeEndDate,ProjectPlannedEndDate,BreakStartDate,BreakEndDate,BadgeStartDateSource,BadgeEndDateSource,PlannedEndDateSource)
	SELECT B.PersonId, CASE	WHEN B.StartDate > @DefaultStartDate THEN B.StartDate 
							ELSE @DefaultStartDate END,
					   CASE WHEN B.BadgeStartDateSource = 'MS Exception' THEN (SELECT ExceptionEndDate FROM MSBadge WHERE PersonId = @PersonId)
							WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate)-1 ELSE B.PlannedEnd END) 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate)-1 ELSE B.PlannedEnd END) END,
					   CASE WHEN B.PlannedEndDateSource ='Manual Entry' THEN NULL ELSE B.PlannedEnd END,
					   CASE WHEN B.BadgeStartDateSource = 'MS Exception' THEN (SELECT ExceptionEndDate+1 FROM MSBadge WHERE PersonId = @PersonId)
							WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,B.StartDate) ELSE B.PlannedEnd+1 END)-- DATEADD(MM,18,B.StartDate) 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,18,@DefaultStartDate) ELSE B.PlannedEnd+1 END) END,
					   CASE	WHEN B.BadgeStartDateSource = 'MS Exception' THEN (SELECT DATEADD(MM,6,ExceptionEndDate) FROM MSBadge WHERE PersonId = @PersonId)
							WHEN B.StartDate > @DefaultStartDate THEN (CASE WHEN DATEADD(MM,18,B.StartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,B.StartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) --DATEADD(MM,24,B.StartDate)-1 
							ELSE (CASE WHEN DATEADD(MM,18,@DefaultStartDate)-1 > B.PlannedEnd THEN DATEADD(MM,24,@DefaultStartDate)-1 ELSE DATEADD(MM,6,B.PlannedEnd) END) END,
					   B.BadgeStartDateSource,
					   CASE WHEN B.BadgeStartDateSource = 'MS Exception' THEN 'MS Exception'
							WHEN B.PlannedEnd > DATEADD(MM,18,B.StartDate)-1 AND B.PlannedEnd > DATEADD(MM,18,@DefaultStartDate)-1 THEN B.PlannedEndDateSource
							ELSE B.BadgeStartDateSource END,
					   CASE WHEN B.PlannedEndDateSource ='Manual Entry' THEN NULL ELSE B.PlannedEndDateSource END
	FROM @BadgeTable3 B

	IF NOT EXISTS(SELECT 1 FROM BadgeHistoryForReports WHERE PersonId = @PersonId)
	BEGIN
	  INSERT INTO BadgeHistoryForReports(PersonId,BadgeStartDate,BadgeEndDate,ProjectPlannedEndDate,BreakStartDate,BreakEndDate,BadgeStartDateSource,BadgeEndDateSource,PlannedEndDateSource)
	  SELECT PersonId,BadgeStartDate,BadgeEndDate,PlannedEndDate,BreakStartDate,BreakEndDate,BadgeStartDateSource,BadgeEndDateSource,PlannedEndDateSource
	  FROM MSBadge 
	  WHERE PersonId = @PersonId
	END

END

