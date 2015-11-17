CREATE PROCEDURE [dbo].[InsertProjectFeedbackByMilestonePersonId]
(
	@MilestonePersonId   INT=NULL,
	@MilestoneId	INT=NULL,
	@ProjectId		INT=NULL,
	@PersonId		INT=NULL
)
AS
BEGIN

	DECLARE @ProjectIdLocal INT,
	@PersonIdLocal INT

--	SELECT @ProjectIdLocal = M.ProjectId 
--	FROM Milestone M
--	JOIN MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
--	WHERE (@MilestonePersonId IS NULL OR MP.MilestonePersonId = @MilestonePersonId)
--		  AND (@MilestoneId IS NULL OR M.MilestoneId = @MilestoneId)
	
--	SELECT @PersonIdLocal = PersonId FROM MilestonePerson WHERE MilestonePersonId = @MilestonePersonId

--	DECLARE @Temp TABLE(PersonId INT,ProjectId INT,StartDate DATETIME,EndDate DATETIME,Hours REAL,HoursInRange REAL,GroupBy INT,IsGap BIT)

--	DELETE PF
--	FROM dbo.ProjectFeedback PF
--	LEFT JOIN dbo.MilestonePerson MP ON MP.PersonId = PF.PersonId
--	LEFT JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId 
--	WHERE (@MilestonePersonId IS NULL OR (PF.personid = @PersonIdLocal and PF.ProjectId = @ProjectIdLocal))
--		  AND (@MilestoneId IS NULL OR PF.ProjectId = @ProjectIdLocal)
--		  AND (@ProjectId  IS NULL OR PF.ProjectId = @ProjectId)
--		  AND (@PersonId IS NULL OR PF.PersonId = @PersonId)
--		  AND (PF.FeedbackStatusId <> 1 AND PF.IsCanceled = 0)

--	EXEC [dbo].[InsertFeedbackByWeekWise] @MilestonePersonId = @MilestonePersonId, @MilestoneId =@MilestoneId, @ProjectId = @ProjectId

--		Declare @PersonIdC INT
--		Declare @ProjectIdC INT
--		Declare @NextPersonId INT
--		Declare @NextProjectId INT
--		Declare @startDate DATETIME
--		Declare @TempStartDate DATETIME
--		Declare @beforestopDate DATETIME
--		Declare @stopDate DATETIME
--		Declare @SummedHours	real
--		Declare @Hours	real
--		DECLARE @ChangeStartDateFlag BIT=0
--		DECLARE @HoursInRange REAL
--		DECLARE @IsGap	BIT=0
--		DECLARE @Groupby INT=0
--		DECLARE cursor_temp CURSOR -- Declare and initialise cursor

--		LOCAL SCROLL STATIC
--		FOR
--		 WITH MilestoneRecordsContinuity
--			AS
--			(   
--				SELECT distinct Per.PersonId,m.ProjectId,dbo.GreaterDateBetweenTwo(mpe.StartDate,pay.StartDate) AS StartDate,dbo.SmallerDateBetweenTwo(mpe.EndDate,pay.EndDateOrig-1) AS EndDate--,ROW_NUMBER() over(partition by Per.PersonId,m.ProjectId order by mpe.StartDate) as RNo
--				FROM dbo.Person Per
--				JOIN MilestonePerson mp on mp.Personid = Per.PersonId
--				JOIN Milestone m on m.MilestoneId = mp.MilestoneId
--				JOIN MilestonePersonEntry mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
--				JOIN dbo.Project P ON P.ProjectId = M.ProjectId
--				INNER JOIN dbo.Title T ON T.TitleId = Per.TitleId
--				INNER JOIN v_Pay pay ON pay.PersonId = Per.PersonId AND MPE.StartDate <= pay.EndDateOrig-1 AND pay.StartDate <= MPE.EndDate
--				AND (@MilestonePersonId IS NULL OR (mp.personid = @PersonIdLocal and m.ProjectId = @ProjectIdLocal))
--				AND (@MilestoneId IS NULL OR M.ProjectId = @ProjectIdLocal)
--				AND (@ProjectId  IS NULL OR M.ProjectId = @ProjectId)
--				AND (@PersonId IS NULL OR MP.PersonId = @PersonId)
--				AND Per.IsStrawman = 0
--				AND T.Title NOT IN ('Senior Director','Client Director','Practice Director')
--				AND pay.Timescale IN (1,2,3,4) -- AS PER E00053
--				AND M.ProjectId <> 174
--				AND P.IsAllowedToShow = 1
--			)
--			,
--			PersonHoursByDay
--			AS
--			(
--				select MRC.PersonId,MRC.ProjectId,MPS.Date,SUM(MPS.HoursPerDay) AS Hours from MilestoneRecordsContinuity as mrc
--				join v_MilestonePersonSchedule as mps on mps.ProjectId = mrc.ProjectId and mps.PersonId = mrc.PersonId
--				where mps.Date BETWEEN mrc.StartDate AND mrc.EndDate
--				GROUP BY MRC.PersonId,MRC.ProjectId,MPS.Date
--			)
--			SELECT PHD.PersonId,PHD.ProjectId,PHD.Date,PHD.Hours
--			FROM PersonHoursByDay PHD
--			LEFT JOIN dbo.ProjectFeedback PF ON PF.PersonId = PHD.PersonId AND PF.ProjectId = PHD.ProjectId AND PHD.Date BETWEEN PF.ReviewPeriodStartDate AND PF.ReviewPeriodEndDate AND (PF.FeedbackStatusId = 1 OR PF.IsCanceled = 1)
--			WHERE PF.FeedbackId IS NULL
--			AND PHD.Date >= '20150101'
--			ORDER BY PHD.PersonId,PHD.ProjectId,PHD.Date

--		OPEN cursor_temp

--		FETCH NEXT FROM cursor_temp INTO
--		@PersonIdC, @ProjectIdC, @startDate,@SummedHours

--		SET @beforestopDate = @startDate
--		SET @TempStartDate = @startDate
--		SET @HoursInRange = @SummedHours

--		 FETCH NEXT FROM cursor_temp INTO
--		@NextPersonId,@NextProjectId,@stopDate,@Hours

--WHILE @@Fetch_status = 0 --SUCCESS
--BEGIN
--    IF @ProjectIdC = @NextProjectId
--	BEGIN
--		IF @PersonIdC = @NextPersonId
--		BEGIN
--		IF DATEDIFF(DAY, @beforestopDate, @stopDate) >= 42
--		BEGIN
--			INSERT INTO @Temp
--			SELECT @PersonIdC
--				,@ProjectIdC
--				,@startDate
--				,@beforestopDate
--				,@SummedHours
--				,@HoursInRange
--				,@Groupby
--				,@IsGap

--			SET @PersonIdC = @NextPersonId
--			SET @ProjectIdC = @NextProjectId
--			SET @startDate = @stopDate
--			SET @beforestopDate = @stopDate
--			SET @SummedHours = @Hours
--			SET @HoursInRange = @Hours
--			SET @IsGap = 0
--			SET @Groupby = @Groupby + 1

--			FETCH NEXT
--			FROM cursor_temp
--			INTO @NextPersonId
--				,@NextProjectId
--				,@stopDate
--				,@Hours
--		END
--		ELSE
--		BEGIN
--			IF (@IsGap = 0)
--				AND DATEDIFF(DAY, @beforestopDate, @stopDate) > 1
--			BEGIN
--				SET @IsGap = 1
--			END

--			IF (FLOOR(@SummedHours / 520) <> FLOOR((@SummedHours + @Hours) / 520))
--			BEGIN
--				SET @ChangeStartDateFlag = 1

--				IF @SummedHours + @Hours >= 520
--					AND @HoursInRange + @Hours >= 240
--				BEGIN
--					SET @Groupby = @Groupby + 1
--				END

--				INSERT INTO @Temp
--				SELECT @PersonIdC
--				    ,@ProjectIdC
--					,@TempStartDate
--					,@stopDate
--					,@SummedHours + @Hours
--					,@HoursInRange + @Hours
--					,@Groupby
--					,@IsGap

--				SET @HoursInRange = 0
--				SET @IsGap = 0
--			END

--			SET @beforestopDate = @stopDate
--			SET @SummedHours = @SummedHours + @Hours
--			SET @HoursInRange = @HoursInRange + @Hours

--			FETCH NEXT
--			FROM cursor_temp
--			INTO @NextPersonId
--				,@NextProjectId
--				,@stopDate
--				,@Hours

--			IF (@ChangeStartDateFlag = 1)
--			BEGIN
--				SET @TempStartDate = @stopDate
--				SET @startDate = @stopDate
--				SET @ChangeStartDateFlag = 0
--			END
--		END
--	END
--		ELSE
--		BEGIN
--		IF @SummedHours >= 520
--			AND @HoursInRange >= 240
--		BEGIN
--			SET @Groupby = @Groupby + 1
--		END

--		INSERT INTO @Temp
--		SELECT @PersonIdC
--		    ,@ProjectIdC
--			,@TempStartDate
--			,@beforestopDate
--			,@SummedHours
--			,@HoursInRange
--			,@Groupby
--			,@IsGap

--		SET @Groupby = @Groupby + 1
--		SET @PersonIdC = @NextPersonId
--		SET @ProjectIdC = @NextProjectId
--		SET @startDate = @stopDate
--		SET @TempStartDate = @stopDate
--		SET @beforestopDate = @stopDate
--		SET @SummedHours = @Hours
--		SET @HoursInRange = @Hours
--		SET @IsGap = 0

--		FETCH NEXT
--		FROM cursor_temp
--		INTO @NextPersonId
--			,@NextProjectId
--			,@stopDate
--			,@Hours
--	END
--	END
--	ELSE
--	BEGIN
--		IF @SummedHours >= 520
--			AND @HoursInRange >= 240
--		BEGIN
--			SET @Groupby = @Groupby + 1
--		END

--		INSERT INTO @Temp
--		SELECT @PersonIdC
--		    ,@ProjectIdC
--			,@TempStartDate
--			,@beforestopDate
--			,@SummedHours
--			,@HoursInRange
--			,@Groupby
--			,@IsGap

--		SET @Groupby = @Groupby + 1
--		SET @PersonIdC = @NextPersonId
--		SET @ProjectIdC = @NextProjectId
--		SET @startDate = @stopDate
--		SET @TempStartDate = @stopDate
--		SET @beforestopDate = @stopDate
--		SET @SummedHours = @Hours
--		SET @HoursInRange = @Hours
--		SET @IsGap = 0

--		FETCH NEXT
--		FROM cursor_temp
--		INTO @NextPersonId
--			,@NextProjectId
--			,@stopDate
--			,@Hours
--	END
--END

--IF @SummedHours >= 520
--	AND @HoursInRange >= 240
--BEGIN
--	SET @Groupby = @Groupby + 1
--END
--		INSERT INTO @Temp
--		SELECT @PersonIdC,@ProjectIdC,@startDate,@beforestopDate,@SummedHours,@HoursInRange,@Groupby,@IsGap

--		CLOSE cursor_temp
--		DEALLOCATE cursor_temp

--		INSERT INTO dbo.ProjectFeedback(ProjectId,PersonId,ReviewPeriodStartDate,ReviewPeriodEndDate,DueDate,FeedbackStatusId,IsCanceled,CompletionCertificateBy,CompletionCertificateDate,CancelationReason,NextIntialMailSendDate,IsGap)
--		SELECT MAX(ProjectId),
--			   Max(PersonId),
--			   MIN(StartDate),
--			   MAX(EndDate), 
--			   DATEADD(WEEK,2,MAX(EndDate)),
--			   2,
--			   0,
--			   NULL,
--			   NULL,
--			   NULL,
--			   MAX(EndDate),
--			   MAX(CAST(IsGap  AS INT)) 
--		FROM @Temp 
--		WHERE Hours >= 240
--		GROUP BY GroupBy
END

