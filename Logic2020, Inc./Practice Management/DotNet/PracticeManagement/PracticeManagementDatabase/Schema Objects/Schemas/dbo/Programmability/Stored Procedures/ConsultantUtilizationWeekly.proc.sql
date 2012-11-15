CREATE PROCEDURE [dbo].[ConsultantUtilizationWeekly]
    @StartDate DATETIME,
    @Step INT = 7,
    @DaysForward INT = 184,
    @ActivePersons BIT = 1,
    @ActiveProjects BIT = 1,
    @ProjectedPersons BIT = 1,
    @ProjectedProjects BIT = 1,
    @ExperimentalProjects BIT = 1,
	@InternalProjects	BIT = 1,
	@TimescaleIds NVARCHAR(4000) = NULL,
	@PracticeIds NVARCHAR(4000) = NULL,
	@ExcludeInternalPractices BIT = 0,
	@SortId INT = 0,
	@SortDirection NVARCHAR(15) = 'DESC',
	@IsSampleReport BIT = 0
	
AS 
   BEGIN
        SET NOCOUNT ON ;
        IF (@IsSampleReport = 1)
        BEGIN
		SELECT @PracticeIds = COALESCE(@PracticeIds+',' ,'') + Convert(VARCHAR,PracticeId)
		FROM Practice
		ORDER BY Name
		SET @PracticeIds = ','+@PracticeIds+','
        END
        
        DECLARE @OrderBy NVARCHAR(4000),@Query NVARCHAR(4000)
        SET @OrderBy = ' ORDER BY    '
        
		IF(@SortId = 1) --Alphabetical  Last name
        BEGIN
			SET @OrderBy = @OrderBy + ' p.LastName ' + @SortDirection
        END
        ELSE IF(@SortId = 2) --Alphabetical  Pay Type
        BEGIN
			SET @OrderBy = @OrderBy + ' paytp.[Name] DESC' +
							', wutilAvg  ASC'
        END
        ELSE IF(@SortId = 3) --Alphabetical  Practice
        BEGIN
			SET @OrderBy = @OrderBy + ' pr.[Name] DESC'  +
						   ', wutilAvg  ASC'
        END
        ELSE
        BEGIN  --Average Utilization for Period
        
			SET @OrderBy = @OrderBy + 'wutilAvg '
			SET @OrderBy = @OrderBy + @SortDirection + ' ,  p.LastName DESC'
        END
        
        SET @Query = ' DECLARE @EndDate DATETIME
					   SET @EndDate = DATEADD(DAY, @DaysForward - 1, @StartDate) '

		/*
		----- Person Status ------
		1	Active
		2	Terminated
		3	Projected
		4	Inactive
		*/

    ---------------------------------------------------------
    -- Retrieve all consultants working at current month
		SET @Query = @Query + '
        DECLARE @CurrentConsultants TABLE ( ConsId INT, TimeScaleId INT, TimeScaleName NVARCHAR(50) ) ;
        INSERT  INTO @CurrentConsultants ( ConsId, TimeScaleId, TimeScaleName )
                SELECT  p.PersonId, T.TimescaleId, T.Name
                FROM    dbo.Person AS p
				INNER JOIN dbo.Timescale T ON T.TimescaleId IN ( SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@TimescaleIds))
				INNER JOIN dbo.GetCurrentPayTypeTable() AS PCPT ON PCPT.PersonId = p.PersonId AND T.TimescaleId = PCPT.Timescale  
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
                WHERE   (p.IsStrawman = 0) 
                        AND ( (@ActivePersons = 1 AND p.PersonStatusId IN (1,5)) 
								OR
                              (@ProjectedPersons = 1 AND p.PersonStatusId = 3)
							)
						AND (					
								p.DefaultPractice IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))
							 AND (pr.IsCompanyInternal = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)				
							) '
			
				
	-- @CurrentConsultants now contains ids of consultants
    ---------------------------------------------------------
    
    SET @Query = @Query + '
        SELECT  p.PersonId,
                p.EmployeeNumber,
                p.FirstName,
                p.LastName,
                p.HireDate,
				p.TerminationDate,
                c.TimescaleId,
                c.[TimeScaleName] AS Timescale,
				st.PersonStatusId,
                st.[Name],
				S.[Name] Seniorityname,
				S.SeniorityId,
                --dbo.GetWeeklyUtilization(c.ConsId, @StartDate, @Step, @DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects) AS wutil,
                AvgUT.AvgUtilization AS wutilAvg,
                ISNULL(VactionDaysTable.VacationDays,0) AS PersonVactionDays
        FROM    dbo.Person AS p
                INNER JOIN @CurrentConsultants AS c ON c.ConsId = p.PersonId
                INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
				INNER JOIN dbo.Seniority S ON P.SeniorityId = S.SeniorityId
				INNER JOIN dbo.GetNumberAvaliableHoursTable(@StartDate,@EndDate,@ActiveProjects,@ProjectedProjects,@ExperimentalProjects,@InternalProjects) AS AvaHrs ON AvaHrs.PersonId =  p.PersonId AND AvaHrs.AvaliableHours > 0
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
                LEFT JOIN dbo.GetPersonVacationDaysTable(@StartDate,@Enddate) VactionDaysTable ON VactionDaysTable.PersonId = c.ConsId
				LEFT JOIN dbo.GetAvgUtilizationTable(@StartDate,@EndDate,@ActiveProjects,@ProjectedProjects,@ExperimentalProjects,@InternalProjects) AS AvgUT ON AvgUT.PersonId =  p.PersonId'

     SET @Query = @Query+@OrderBy
	SET @Query = @Query+	
		'  
		SELECT	PH.PersonId,
				PH.HireDate,
				PH.TerminationDate
		FROM v_PersonHistory PH
		INNER JOIN @CurrentConsultants AS c ON c.ConsId = PH.PersonId
		ORDER BY PH.PersonId,PH.HireDate
				 
		'
     
     --PRINT @Query
     EXEC SP_EXECUTESQL @Query,N'@StartDate				DATETIME,
								 @Step					INT ,
								 @DaysForward			INT ,
								 @ActivePersons			BIT,
								 @ActiveProjects		BIT,
								 @ProjectedPersons		BIT,
								 @ProjectedProjects		BIT,
								 @ExperimentalProjects	BIT,
								 @InternalProjects		BIT,
								 @TimescaleIds			NVARCHAR(4000),
								 @PracticeIds			NVARCHAR(4000),
								 @ExcludeInternalPractices	BIT',
								 @StartDate		=	@StartDate,
								 @Step			=   @Step,
								 @DaysForward	=	@DaysForward,
								 @ActivePersons	=	@ActivePersons,
								 @ActiveProjects	=	@ActiveProjects,
								 @ProjectedPersons	=	@ProjectedPersons,
								 @ProjectedProjects	=	@ProjectedProjects,
								 @ExperimentalProjects = @ExperimentalProjects,
								 @InternalProjects	=	@InternalProjects,
								 @TimescaleIds	=	@TimescaleIds,
								 @PracticeIds	=	@PracticeIds,
								 @ExcludeInternalPractices = @ExcludeInternalPractices
     
    END

