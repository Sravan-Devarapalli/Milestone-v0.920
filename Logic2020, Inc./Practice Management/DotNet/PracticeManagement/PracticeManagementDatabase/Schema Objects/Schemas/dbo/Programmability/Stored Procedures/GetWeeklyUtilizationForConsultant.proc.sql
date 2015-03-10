CREATE PROCEDURE [dbo].[GetWeeklyUtilizationForConsultant]
    @StartDate DATETIME,
    @Step INT = 7,
    @DaysForward INT = 184,
    @ActivePersons BIT = 1,
    @ActiveProjects BIT = 1,
    @ProjectedPersons BIT = 1,
    @ProjectedProjects BIT = 1,
    @ExperimentalProjects BIT = 1,
	@ProposedProjects BIT = 1,
	@InternalProjects	BIT = 1,
	@CompletedProjects	BIT = 1,
	@TimescaleIds NVARCHAR(4000) = NULL,
	@PracticeIds NVARCHAR(4000) = NULL,
	@ExcludeInternalPractices BIT = 0,
	@IsSampleReport BIT = 0,
	@UtilizationType BIT = 0, -- 0 for only utilization, 1 for project utilization, 2 for hours utilization
	@IsBadgeIncluded BIT=0
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
			IF(DATEPART(DW,@StartDate)>0)
			BEGIN
				SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+1
			END
			IF(DATEPART(DW,@StartDate)<7)
			BEGIN
				SELECT @EndRange = DATEADD(dd , 7-DATEPART(DW,@EndRange), @EndRange)
			END
		END
		ELSE IF (@Step = 30)
		BEGIN
                
			IF(DATEPART(DW,@StartDate)>0)
			BEGIN
				SELECT @StartDate = @StartDate - DATEPART(DW,@StartDate)+1
			END
			IF(DATEPART(DW,@EndRange)<7)
			BEGIN
				SELECT @EndRange = DATEADD(dd , 7-DATEPART(DW,@EndRange), @EndRange)
			END
		END

		IF(@UtilizationType = 0)
		BEGIN
			SELECT WUT.PersonId,WUT.WeeklyUtlization,WUT.AvailableHours,WUT.ProjectedHours,WUT.Timescale,WUT.VacationDays
			FROM dbo.GetWeeklyUtilizationTable(@StartDate,@EndRange, @Step, @ActivePersons, @ActiveProjects, @ProjectedPersons, @ProjectedProjects,@ExperimentalProjects,@ProposedProjects,@InternalProjects,@CompletedProjects,@TimescaleIds,@PracticeIds,@ExcludeInternalPractices) AS WUT 
			ORDER BY WUT.PersonId,WUT.StartDate
		END
		IF(@UtilizationType = 1)
		BEGIN
			SELECT WUT.PersonId,WUT.StartDate,WUT.EndDate,WUT.ProjectId,WUT.ProjectName,WUT.ProjectNumber,WUT.WeeklyUtlization,WUT.AvailableHours,WUT.ProjectedHours,WUT.Timescale,WUT.VacationDays
			FROM dbo.GetWeeklyUtilizationByProjectTable(@StartDate,@EndRange, @Step, @ActivePersons, @ActiveProjects, @ProjectedPersons, @ProjectedProjects,@ExperimentalProjects,@ProposedProjects,@InternalProjects,@CompletedProjects,@TimescaleIds,@PracticeIds,@ExcludeInternalPractices) AS WUT 
			ORDER BY WUT.PersonId,WUT.StartDate
		END

		IF(@IsBadgeIncluded = 1)
		BEGIN

			SELECT	MP.PersonId,
					MPE.BadgeStartDate,
					MPE.BadgeEndDate
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE MPE.IsBadgeRequired = 1
			AND P.ClientId = 2 -- ClientId = 2 for MICROSOFT
			AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL

			SELECT PersonId,
				   LastBadgeStartDate,
				   LastBadgeEndDate
			FROM dbo.MSBadge 
			WHERE IsPreviousBadge = 1

		END 
    END

