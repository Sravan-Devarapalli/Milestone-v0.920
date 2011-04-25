CREATE PROCEDURE dbo.ConsultantUtilizationDailyByPerson
    @StartDate DATETIME,
    @DaysForward INT = 184,
    @ActiveProjects BIT = 1,
    @ProjectedProjects BIT = 1,
	@InternalProjects BIT = 1,
    @ExperimentalProjects BIT = 1,
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
                st.[Name],
                dbo.GetWeeklyUtilization(@PersonId, @StartDate, 1, @DaysForward, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects) AS wutil
        FROM    dbo.Person AS p
                INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
                INNER JOIN dbo.Timescale AS paytp ON paytp.TimescaleId = dbo.GetCurrentPayType(@PersonId)
                LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
        WHERE p.PersonId  = @PersonId        
    END

