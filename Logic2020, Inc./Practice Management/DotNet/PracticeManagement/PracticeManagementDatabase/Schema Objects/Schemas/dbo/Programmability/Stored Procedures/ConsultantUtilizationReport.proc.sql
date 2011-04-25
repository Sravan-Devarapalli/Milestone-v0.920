-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-27
-- Description:	Generates consultants report
-- =============================================
CREATE PROCEDURE dbo.ConsultantUtilizationReport
	-- Add the parameters for the stored procedure here
    @startDate DATETIME,
    @endDate DATETIME,
    @ActivePersons BIT = 1,
    @ActiveProjects BIT = 1,
    @ProjectedPersons BIT = 1,
    @ProjectedProjects BIT = 1,
	@InternalProjects BIT = 1,
    @ExperimentalProjects BIT = 1
AS 
    BEGIN
        SET NOCOUNT ON ;
        
        -- Sunday is day one
        SET DATEFIRST 7;
        
        -- Set constant for consultant role id
        DECLARE @ConsRoleId  INT 
        SET @ConsRoleId = 1
        
        ---------------------------------------------------------
        -- Calculate number of company holidays for given period
        DECLARE @CompHolidays INT;
        
        SELECT @CompHolidays = COUNT(*) 
        FROM dbo.v_PersonCalendar
        WHERE CompanyDayOff = 1 AND 
			  DATEPART(dw, Date) != 7 AND 
			  DATEPART(dw, Date) != 1 AND 
			  Date BETWEEN @startDate AND @endDate
			  
		-- @CompHolidays calculation finished
		----------------------------------------------------------
		
		
        ---------------------------------------------------------
        -- Retrieve all consultants working at current month
		DECLARE @CurrentConsultants TABLE(ConsId INT);
		INSERT INTO @CurrentConsultants (ConsId) 
			SELECT p.PersonId
			FROM dbo.Person AS p
			LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
			WHERE ISNULL(pr.IsCompanyInternal, 0) = 0 AND 
			(
				p.PersonStatusId = CASE WHEN @ProjectedPersons = 1 THEN 3 ELSE 0 END
				OR 
				p.PersonStatusId = CASE WHEN @ActivePersons = 1 THEN 1 ELSE 0 END
			)
			
		-- @CurrentConsultants now contains ids of consultants
        ---------------------------------------------------------
           
	;with PersonLoad as (
		SELECT
		   pers.PersonId,
		   pers.EmployeeNumber,
           pers.LastName + ', ' + pers.FirstName AS 'Name', 
           sen.[Name] AS SeniorityName,
           tsc.[Name] AS PayType,
           tsc.TimescaleId,
           dbo.GetNumberWorkingDays(cons.ConsId, @startDate, @endDate) AS WorkingDays,
           dbo.GetNumberAvaliableHours(cons.ConsId, @startDate, @endDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects) AS AvailableHours,
           dbo.GetNumberHolidayDays(cons.ConsId, @startDate, @endDate)*8  AS HolidayHours,
		   dbo.GetNumberProjectedHours(cons.ConsId, @startDate, @endDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects) AS RawProjectedHours,
           ISNULL(dbo.GetNumberProjectedHours(cons.ConsId, @startDate, @endDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects), 0) AS ProjectedHours
		FROM dbo.Person AS pers
		INNER JOIN dbo.Seniority AS sen ON pers.SeniorityId = sen.SeniorityId
		INNER JOIN @CurrentConsultants AS cons ON cons.ConsId = pers.PersonId
		INNER JOIN dbo.Timescale AS tsc ON tsc.TimescaleId = dbo.GetCurrentPayType(pers.PersonId)
		WHERE dbo.GetNumberAvaliableHours(cons.ConsId, @startDate, @endDate, @ActiveProjects, @ProjectedProjects, @ExperimentalProjects,@InternalProjects) > 0
	) 
	SELECT 
		   pl.PersonId,
		   pl.EmployeeNumber AS 'Consultant #',
           pl.Name, 
           pl.SeniorityName,
           pl.PayType AS 'Pay Type',
           pl.TimescaleId,
           pl.WorkingDays AS 'Working days',
           pl.AvailableHours AS 'Available Hours',
           pl.HolidayHours  AS 'Holiday Hours',
           pl.ProjectedHours AS 'Projected Hours',
           CASE 
			WHEN 
				pl.RawProjectedHours IS NULL OR (pl.AvailableHours <= pl.RawProjectedHours + pl.HolidayHours) THEN 0
			ELSE 
				(pl.AvailableHours - pl.HolidayHours - pl.RawProjectedHours)
           END AS 'Bench Hours',
           CEILING(100*pl.ProjectedHours / pl.AvailableHours) AS 'Utilization %'
	FROM PersonLoad as pl
	ORDER BY 
		pl.[PayType] DESC,
		CASE 
			WHEN pl.TimescaleId = 1 OR pl.TimescaleId = 2 
				THEN CEILING(100*pl.ProjectedHours / pl.AvailableHours)
			ELSE pl.ProjectedHours
		END DESC
END

