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
		SELECT @PracticeIds = COALESCE(@PracticeIds+',' ,'') + Convert(varchar,PracticeId)
		FROM Practice
		order by Name
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
							', (dbo.GetAvgUtilization(c.ConsId, @StartDate,@DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects)) ASC'
        END
        ELSE IF(@SortId = 3) --Alphabetical  Practice
        BEGIN
			SET @OrderBy = @OrderBy + ' pr.[Name] DESC'  +
						   ', (dbo.GetAvgUtilization(c.ConsId, @StartDate,@DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects)) ASC'
        END
        ELSE
        BEGIN  --Average Utilization for Period
        
			SET @OrderBy = @OrderBy + '(dbo.GetAvgUtilization(c.ConsId, @StartDate,@DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects))'
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
        DECLARE @CurrentConsultants TABLE ( ConsId INT ) ;
        INSERT  INTO @CurrentConsultants ( ConsId )
                SELECT  p.PersonId
                FROM    dbo.Person AS p
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
                WHERE
                        ( @ActivePersons = 1 AND p.PersonStatusId = 1 OR
                              @ProjectedPersons = 1 AND p.PersonStatusId = 3)
						AND (					
								p.DefaultPractice IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@PracticeIds))
							 AND (ISNULL(pr.IsCompanyInternal, 0) = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)				
							)
						AND (dbo.GetCurrentPayType(p.PersonId) IN ( SELECT ResultId FROM [dbo].[ConvertStringListIntoTable](@TimescaleIds))) '
			
				
	-- @CurrentConsultants now contains ids of consultants
    ---------------------------------------------------------
    
    SET @Query = @Query + ' 
        SELECT  p.PersonId,
                p.EmployeeNumber,
                p.FirstName,
                p.LastName,
                p.HireDate,
                paytp.TimescaleId,
                paytp.[Name] AS Timescale,
                st.[Name],
                dbo.GetWeeklyUtilization(c.ConsId, @StartDate, @Step, @DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects) AS wutil,
                dbo.GetAvgUtilization(c.ConsId, @StartDate,@DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects) AS wutilAvg
        FROM    dbo.Person AS p
                INNER JOIN @CurrentConsultants AS c ON c.ConsId = p.PersonId
                INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
                INNER JOIN dbo.Timescale AS paytp ON paytp.TimescaleId = dbo.GetCurrentPayType(c.ConsId)
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
        WHERE dbo.GetNumberAvaliableHours(c.ConsId, @StartDate, @EndDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects, @InternalProjects) > 0 '
     SET @Query = @Query+@OrderBy
     
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
