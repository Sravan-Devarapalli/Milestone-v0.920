CREATE PROCEDURE dbo.ConsultantUtilizationDailyByPerson
    @StartDate DATETIME,
    @DaysForward INT = 184,
    @ActiveProjects BIT = 1,
    @ProjectedProjects BIT = 1,
	@InternalProjects BIT = 1,
    @ExperimentalProjects BIT = 1,
	@ProposedProjects BIT = 1,
	@CompletedProjects BIT = 1,
	@AtRiskProjects BIT =1,
	@PersonId	INT
AS 
    BEGIN
        SET NOCOUNT ON ;

        DECLARE @EndDate DATETIME
        SET @EndDate = DATEADD(DAY, @DaysForward, @StartDate)

/*
----- Person Status ------
1	Active
2	Terminated
3	Projected
4	Inactive
*/

	-- @CurrentConsultants now contains ids of consultants
    ---------------------------------------------------------
    
       SELECT  p.PersonId,
                p.EmployeeNumber,
                p.FirstName,
                p.LastName,
                p.HireDate,
                paytp.TimescaleId,
                paytp.[Name] AS Timescale,
				st.PersonStatusId,
                st.[Name],
                dbo.GetWeeklyUtilization(@PersonId, @StartDate, 1, @DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@ProposedProjects,@InternalProjects,@CompletedProjects,@AtRiskProjects) AS wutil,
				0 AS wutilAvg
        FROM    dbo.Person AS p
                INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
                INNER JOIN dbo.Timescale AS paytp ON paytp.TimescaleId = dbo.GetCurrentPayType(@PersonId)
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
        WHERE p.PersonId  = @PersonId     
		
		SELECT	PC.PersonId,PC.Date,
				CASE WHEN PC.DayOff=1 AND PC.CompanyDayOff=0 THEN 1
				ELSE 0 END AS IsTimeOff,Cal.HolidayDescription,
				ROUND(ISNULL(PC.TimeOffHours,0),2) TimeOffHours
		FROM dbo.PersonCalendarAuto PC 
		LEFT JOIN dbo.Calendar AS Cal ON Cal.Date=PC.Date
		WHERE PC.PersonId = @PersonId AND
			  PC.[Date] BETWEEN @StartDate AND @EndDate AND
			  PC.DayOff=1 AND (PC.TimeOffHours>0 OR PC.CompanyDayOff=1) AND DATEPART(DW,PC.Date) NOT IN (1,7)
		ORDER BY PC.PersonId,PC.Date   
    END

