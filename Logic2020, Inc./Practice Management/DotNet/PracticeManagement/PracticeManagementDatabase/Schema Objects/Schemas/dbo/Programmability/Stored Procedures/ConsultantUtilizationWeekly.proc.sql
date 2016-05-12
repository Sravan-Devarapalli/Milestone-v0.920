CREATE PROCEDURE [dbo].[ConsultantUtilizationWeekly] @StartDate DATETIME
	,@Step INT = 7
	,@DaysForward INT = 184
	,@ActivePersons BIT = 1
	,@ActiveProjects BIT = 1
	,@ProjectedPersons BIT = 1
	,@ProjectedProjects BIT = 1
	,@ProposedProjects BIT = 1
	,@CompletedProjects BIT = 1
	,@ExperimentalProjects BIT = 1
	,@InternalProjects BIT = 1
	,@TimescaleIds NVARCHAR(4000) = NULL
	,@PracticeIds NVARCHAR(4000) = NULL
	,@ExcludeInternalPractices BIT = 0
	,@SortId INT = 0
	,@SortDirection NVARCHAR(15) = 'DESC'
	,@IsSampleReport BIT = 0
	,@DivisionIds NVARCHAR(4000) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@IsSampleReport = 1)
	BEGIN
		SELECT @PracticeIds = COALESCE(@PracticeIds + ',', '') + Convert(VARCHAR, PracticeId)
		FROM Practice
		ORDER BY Name

		SET @PracticeIds = ',' + @PracticeIds + ','
	END

	DECLARE @OrderBy NVARCHAR(4000)
		,@Query NVARCHAR(MAX)

	SET @OrderBy = ' ORDER BY    '

	IF (@SortId = 1) --Alphabetical  Last name
	BEGIN
		SET @OrderBy = @OrderBy + ' p.LastName ' + @SortDirection
	END
	ELSE IF (@SortId = 2) --Alphabetical  Pay Type
	BEGIN
		SET @OrderBy = @OrderBy + ' c.[TimeScaleName] ASC' + ', wutilAvg  ASC'
	END
	ELSE IF (@SortId = 3) --Alphabetical  Practice
	BEGIN
		SET @OrderBy = @OrderBy + ' pr.[Name] DESC' + ', wutilAvg  ASC'
	END
	ELSE
	BEGIN --Average Utilization for Period
		SET @OrderBy = @OrderBy + 'wutilAvg '
		SET @OrderBy = @OrderBy + @SortDirection + ' ,  p.LastName DESC'
	END

	SET @Query = ' DECLARE @EndDate DATETIME SET @EndDate = DATEADD(DAY, @DaysForward - 1, @StartDate) 
					   IF(@Step = 7) BEGIN
							IF(DATEPART(DW,@StartDate)>0) BEGIN SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+1 END
							IF(DATEPART(DW,@StartDate)<7) BEGIN SELECT @EndDate = DATEADD(dd , 7-DATEPART(DW,@EndDate), @EndDate) END END
						ELSE IF (@Step = 30) BEGIN
							IF(DATEPART(DW,@StartDate)>0) BEGIN SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+1 END
							IF(DATEPART(DW,@EndDate)<7) BEGIN SELECT @EndDate = DATEADD(dd , 7-DATEPART(DW,@EndDate), @EndDate) END END
					   '
	/*
		----- Person Status ------
		1	Active
		2	Terminated
		3	Projected
		4	Inactive
		*/
	---------------------------------------------------------
	-- Retrieve all consultants working at current month
	SET @Query = @Query + 
		'
        DECLARE @CurrentConsultants TABLE (ConsId INT,TimeScaleId INT,TimeScaleName NVARCHAR(50));
        INSERT INTO @CurrentConsultants(ConsId,TimeScaleId,TimeScaleName)
        SELECT p.PersonId,T.TimescaleId,T.Name FROM dbo.Person AS p
		INNER JOIN dbo.Timescale T ON T.TimescaleId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@TimescaleIds))
		INNER JOIN dbo.GetLatestPayWithInTheGivenRange(@StartDate,@EndDate) AS PCPT ON PCPT.PersonId = p.PersonId AND T.TimescaleId = PCPT.Timescale  
        LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
		LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = P.TerminationReasonId
        WHERE (p.IsStrawman = 0) 
						AND (TR.TerminationReasonId IS NULL OR TR.IsPersonWorkedRule = 1)
                        AND ((@ActivePersons = 1 AND p.PersonStatusId IN (1,5)) OR (@ProjectedPersons = 1 AND p.PersonStatusId = 3))
						AND (p.DefaultPractice IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PracticeIds)) AND (pr.IsCompanyInternal = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0))
						AND pr.ShowInUtilizationReport = 1 AND p.IsOffshore=0
						AND (p.DivisionId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@DivisionIds))) '
	-- @CurrentConsultants now contains ids of consultants
	---------------------------------------------------------
	SET @Query = @Query + 
		'
        SELECT  p.PersonId,p.EmployeeNumber,p.First AS FirstName,p.LastName,p.HireDate,p.TerminationDate,c.TimescaleId,c.TimeScaleName AS Timescale,st.PersonStatusId,st.Name,P.TitleId,p.Title,p.DefaultPractice PracticeId,p.PracticeName,
				CASE WHEN AvaHrs.AvaliableHours > 0 THEN  AvgUT.AvgUtilization ELSE 0 END AS wutilAvg,ISNULL(VactionDaysTable.VacationDays,0) AS PersonVactionDays,M.BadgeStartDate,M.BadgeEndDate,M.BreakStartDate,M.BreakEndDate,M.BlockStartDate,M.BlockEndDate,B.ManageServiceContract,p.IsInvestmentResource,p.TargetUtilization
		        FROM v_person AS p INNER JOIN @CurrentConsultants AS c ON c.ConsId = p.PersonId
				LEFT JOIN v_CurrentMSBadge M ON M.PersonId = p.PersonId INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
				LEFT JOIN MSBadge B ON B.Personid=p.personid 
				LEFT JOIN dbo.GetNumberAvaliableHoursTable(@StartDate,@EndDate,@ActiveProjects,@ProjectedProjects,@ExperimentalProjects,@InternalProjects,@ProposedProjects,@CompletedProjects) AS AvaHrs ON AvaHrs.PersonId =  p.PersonId 
		LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
                LEFT JOIN dbo.GetPersonVacationDaysTable(@StartDate,@Enddate) VactionDaysTable ON VactionDaysTable.PersonId = c.ConsId
				LEFT JOIN dbo.GetAvgUtilizationTable(@StartDate,@EndDate,@ActiveProjects,@ProjectedProjects,@ExperimentalProjects,@InternalProjects,@ProposedProjects,@CompletedProjects) AS AvgUT ON AvgUT.PersonId =  p.PersonId'
	SET @Query = @Query + @OrderBy
	SET @Query = @Query + '  
		SELECT PH.PersonId,PH.HireDate,PH.TerminationDate FROM v_PersonHistory PH INNER JOIN @CurrentConsultants AS c ON c.ConsId = PH.PersonId
		ORDER BY PH.PersonId,PH.HireDate '
	--if a person has added Timeoff  for complete 8 hr then the day is treated as vacation day.
	SET @Query = @Query + '  
		SELECT	PC.PersonId,PC.Date,CASE WHEN PC.DayOff=1 AND PC.CompanyDayOff=0 THEN 1	ELSE 0 END AS IsTimeOff,Cal.HolidayDescription,ROUND(ISNULL(PC.TimeOffHours,0),2) TimeOffHours
		FROM dbo.PersonCalendarAuto PC INNER JOIN @CurrentConsultants AS c ON c.ConsId=PC.PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
		LEFT JOIN dbo.Calendar AS Cal ON Cal.Date=PC.Date
		WHERE PC.DayOff=1 AND (PC.TimeOffHours>0 OR PC.CompanyDayOff=1) AND DATEPART(DW,PC.Date) NOT IN (1,7)
		ORDER BY PC.PersonId,PC.Date'

	--PRINT @Query
	EXEC SP_EXECUTESQL @Query
		,N'@StartDate				DATETIME,
								 @Step					INT ,
								 @DaysForward			INT ,
								 @ActivePersons			BIT,
								 @ActiveProjects		BIT,
								 @ProjectedPersons		BIT,
								 @ProjectedProjects		BIT,
								 @ProposedProjects		BIT,
								 @ExperimentalProjects	BIT,
								 @InternalProjects		BIT,
								 @CompletedProjects		BIT,
								 @TimescaleIds			NVARCHAR(4000),
								 @PracticeIds			NVARCHAR(4000),
								 @ExcludeInternalPractices	BIT,
								 @DivisionIds			NVARCHAR(4000)'
		,@StartDate = @StartDate
		,@Step = @Step
		,@DaysForward = @DaysForward
		,@ActivePersons = @ActivePersons
		,@ActiveProjects = @ActiveProjects
		,@ProjectedPersons = @ProjectedPersons
		,@ProjectedProjects = @ProjectedProjects
		,@ProposedProjects = @ProposedProjects
		,@ExperimentalProjects = @ExperimentalProjects
		,@InternalProjects = @InternalProjects
		,@CompletedProjects = @CompletedProjects
		,@TimescaleIds = @TimescaleIds
		,@PracticeIds = @PracticeIds
		,@ExcludeInternalPractices = @ExcludeInternalPractices
		,@DivisionIds = @DivisionIds

	DECLARE @EndDateReport DATETIME
		,@StartDateReport DATETIME
   
    SET @StartDateReport = @StartDate
	SET @EndDateReport = DATEADD(DAY, @DaysForward - 1, @StartDate)

	IF (@Step = 7)
	BEGIN
		IF (DATEPART(DW, @StartDate) > 0)
		BEGIN
			SELECT @StartDateReport = @StartDate - DATEPART(DW, @StartDate) + 1
		END

		IF (DATEPART(DW, @StartDate) < 7)
		BEGIN
			SELECT @EndDateReport = DATEADD(dd, 7 - DATEPART(DW, @EndDateReport), @EndDateReport)
		END
	END
	ELSE IF (@Step = 30)
	BEGIN
		IF (DATEPART(DW, @StartDate) > 0)
		BEGIN
			SELECT @StartDateReport = @StartDate - DATEPART(DW, @StartDate) + 1
		END

		IF (DATEPART(DW, @EndDateReport) < 7)
		BEGIN
			SELECT @EndDateReport = DATEADD(dd, 7 - DATEPART(DW, @EndDateReport), @EndDateReport)
		END
	END

	SELECT P.PersonId
		,MPE.StartDate
		,MPE.EndDate
		,Prj.NAME AS ProjectName
		,Prj.ProjectNumber
	FROM dbo.Person AS p
	INNER JOIN dbo.Timescale T ON T.TimescaleId IN (
			SELECT ResultId
			FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
			)
	INNER JOIN dbo.GetLatestPayWithInTheGivenRange(@StartDateReport, @EndDateReport) AS PCPT ON PCPT.PersonId = p.PersonId
		AND T.TimescaleId = PCPT.Timescale
	INNER JOIN dbo.MilestonePerson MP ON MP.PersonId = p.PersonId
	INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project Prj ON Prj.ProjectId = M.ProjectId
	LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
	LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = P.TerminationReasonId
	WHERE (p.IsStrawman = 0)
	
		AND (
			TR.TerminationReasonId IS NULL
			OR TR.IsPersonWorkedRule = 1
			)
		AND (
			(
				@ActivePersons = 1
				AND p.PersonStatusId IN (
					1
					,5
					)
				)
			OR (
				@ProjectedPersons = 1
				AND p.PersonStatusId = 3
				)
			)
		AND (
			p.DefaultPractice IN (
				SELECT ResultId
				FROM dbo.ConvertStringListIntoTable(@PracticeIds)
				)
			AND (
				pr.IsCompanyInternal = 0
				AND @ExcludeInternalPractices = 1
				OR @ExcludeInternalPractices = 0
				)
				AND pr.ShowInUtilizationReport = 1 
				AND p.IsOffshore=0
			)
		AND (
			p.DivisionId IN (
				SELECT ResultId
				FROM dbo.ConvertStringListIntoTable(@DivisionIds)
				)
			)
		AND (
			MPE.StartDate <= @EndDateReport
			AND @StartDateReport <= MPE.EndDate
			)
		AND Prj.ProjectNumber != 'P031000'
		AND Prj.IsAllowedToShow = 1
		AND ((@ActiveProjects=1 AND prj.ProjectStatusId=3) OR
			(@ProjectedProjects=1 AND  Prj.ProjectStatusId=2 ) OR
			(@ProposedProjects=1 AND Prj.ProjectStatusId=7)  OR
			(@CompletedProjects=1 AND  Prj.ProjectStatusId=4 ) OR
			(@ExperimentalProjects=1 AND  Prj.ProjectStatusId=5 ) OR
			(@InternalProjects=1 AND Prj.ProjectStatusId=6))

END

