CREATE PROCEDURE [dbo].[GetWeeklyUtilizationForConsultant]
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
	@IsSampleReport BIT = 0
	
AS 
   BEGIN
   /*
   1.If @IsSampleReport is 1 THEN Populate all practices in @PracticeIds.
   2.If step is 7
	 a.If STARTDATE is SUNDAY then set STARTDATE to MONDAY of the STARTDATE week.
	 b.If STARTDATE is not sat then set ENDDATE to sat of the ENDDATE week.

   3.If step is 30
	 a.If STARTDATE is SUNDAY then set STARTDATE to MONDAY of the STARTDATE week.
	 b.If ENDDATE is not sat then set ENDDATE to sat of the ENDDATE week.
   4.Get the WeeklyUtlization of the person for the given filters.
   */
        SET NOCOUNT ON ;
        IF (@IsSampleReport = 1)
        BEGIN
			SELECT @PracticeIds = COALESCE(@PracticeIds+',' ,'') + Convert(VARCHAR,PracticeId)
			FROM Practice
			ORDER BY Name
			SET @PracticeIds = ','+@PracticeIds+','
        END

		DECLARE @EndRange  DATETIME
		SET @EndRange = DATEADD(dd , @DaysForward, @StartDate) - 1
		IF(@Step = 7)
		BEGIN
			IF(DATEPART(DW,@StartDate)>2)
			BEGIN
				SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+2
			END
			IF(DATEPART(DW,@StartDate)<7)
			BEGIN
				SELECT @EndRange = DATEADD(dd , 7-DATEPART(DW,@EndRange), @EndRange)
			END
		END
		ELSE IF (@Step = 30)
		BEGIN
                
			IF(DATEPART(DW,@StartDate)>2)
			BEGIN
				SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+2
			END
			IF(DATEPART(DW,@EndRange)<7)
			BEGIN
				SELECT @EndRange = DATEADD(dd , 7-DATEPART(DW,@EndRange), @EndRange)
			END
		END

		SELECT WUT.PersonId,WUT.WeeklyUtlization
		FROM dbo.GetWeeklyUtilizationTable(@StartDate,@EndRange, @Step, @ActivePersons, @ActiveProjects, @ProjectedPersons, @ProjectedProjects,@ExperimentalProjects,@InternalProjects,@TimescaleIds,@PracticeIds,@ExcludeInternalPractices) AS WUT 
		ORDER BY WUT.PersonId,WUT.StartDate
				 
    END
